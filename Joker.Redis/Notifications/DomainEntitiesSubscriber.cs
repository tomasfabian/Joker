using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Joker.Contracts;
using Joker.Disposables;
using Joker.Enums;
using Joker.Extensions.Disposables;
using Joker.Factories.Schedulers;
using Joker.Notifications;
using Joker.Redis.ConnectionMultiplexers;
using Newtonsoft.Json;
using SqlTableDependency.Extensions.Notifications;
using StackExchange.Redis;
using ChangeType = TableDependency.SqlClient.Base.Enums.ChangeType;

namespace Joker.Redis.Notifications
{
  public class DomainEntitiesSubscriber<TEntity> : DisposableObject, IDomainEntitiesSubscriber 
    where TEntity : IVersion
  {
    private readonly IRedisSubscriber redisSubscriber;
    private readonly IEntityChangePublisherWithStatus<TEntity> reactiveData;
    private readonly ISchedulersFactory schedulersFactory;

    public DomainEntitiesSubscriber(
      IRedisSubscriber redisSubscriber,
      IEntityChangePublisherWithStatus<TEntity> reactiveData,
      ISchedulersFactory schedulersFactory)
    {
      this.redisSubscriber = redisSubscriber ?? throw new ArgumentNullException(nameof(redisSubscriber));
      this.reactiveData = reactiveData ?? throw new ArgumentNullException(nameof(reactiveData));
      this.schedulersFactory = schedulersFactory ?? throw new ArgumentNullException(nameof(schedulersFactory));

      redisSubscriber.GetStringRetryCount = 3;

      statusChangesSubscription = new SerialDisposable();
      statusChangesSubscription.DisposeWith(CompositeDisposable);
    }

    protected virtual string ChannelName { get; } = typeof(TEntity).Name + "-Changes";
    protected virtual string StatusChannelName { get; } = typeof(TEntity).Name + "-Status";

    #region Methods

    public async Task Subscribe()
    {
      if (IsDisposed)
        throw new ObjectDisposedException("Object has already been disposed.");

      SubscribeToConnectionChanged();

      await redisSubscriber.Subscribe(OnMessageReceived, ChannelName);
    }

    private VersionedTableDependencyStatus lastStatus;

    private void SubscribeToConnectionChanged()
    {
      redisSubscriber.WhenIsConnectedChanges
        .Subscribe(isConnected =>
        {
          if(isConnected)
            SubscribeToStatusChanges();
          else
          {
            statusChangesSubscription.Disposable = Disposable.Empty;
            
            lastStatus = new VersionedTableDependencyStatus(VersionedTableDependencyStatus.TableDependencyStatuses.None, DateTimeOffset.MinValue);

            reactiveData.Publish(lastStatus);
          }
        })
        .DisposeWith(CompositeDisposable);
    }

    private readonly SerialDisposable statusChangesSubscription;

    private void SubscribeToStatusChanges()
    {
      var statusMessagesSource = CreateStatusChangedSource();

      var pullStatusMessageSource = Observable.Defer(() => redisSubscriber.GetStringAsync(StatusChannelName).ToObservable(schedulersFactory.ThreadPool));

      statusChangesSubscription.Disposable =
        statusMessagesSource
          .Merge(pullStatusMessageSource)
          .Where(c => !string.IsNullOrEmpty(c))
          .Select(DeserializeVersionedTableDependencyStatus)
          .Subscribe(status =>
          {
            if(lastStatus == null || status.Timestamp == DateTimeOffset.MinValue || lastStatus.Timestamp < status.Timestamp)
            {
              lastStatus = status; 
            
              reactiveData.Publish(lastStatus);
            }
          });
    }

    internal virtual IObservable<string> CreateStatusChangedSource()
    {
      var statusMessagesSource = Observable.Create<ChannelMessage>(o =>
      {
        redisSubscriber.Subscribe(o.OnNext, StatusChannelName);

        return Disposable.Create(() => redisSubscriber.Unsubscribe(StatusChannelName));
      });

      return statusMessagesSource.Select(OnStatusMessageReceived);
    }

    private string OnStatusMessageReceived(ChannelMessage channelMessage)
    {
      return channelMessage.Message;
    }

    protected virtual VersionedTableDependencyStatus DeserializeVersionedTableDependencyStatus(string message)
    {
      var versionedTableDependencyStatus = JsonConvert.DeserializeObject<VersionedTableDependencyStatus>(message);
      
      return versionedTableDependencyStatus;
    }

    internal void OnStatusMessageReceived(string message)
    {
      var status = DeserializeVersionedTableDependencyStatus(message);
      
      reactiveData.Publish(status);
    }

    private void OnMessageReceived(ChannelMessage channelMessage)
    {
      OnMessageReceived(channelMessage.Message);
    }

    internal void OnMessageReceived(string message)
    {
      var recordChange = DeserializeRecordChangedNotification(message);

      if (recordChange.ChangeType == ChangeType.None) 
        return;

      var entityChange = new EntityChange<TEntity>(recordChange.Entity, Convert(recordChange.ChangeType));

      reactiveData.Publish(entityChange);
    }

    protected virtual RecordChangedNotification<TEntity> DeserializeRecordChangedNotification(string message)
    {
      var recordChange = JsonConvert.DeserializeObject<RecordChangedNotification<TEntity>>(message);
      
      return recordChange;
    }

    private Enums.ChangeType Convert(ChangeType changeType)
    {
      switch (changeType)
      {
        case ChangeType.Insert:
          return Enums.ChangeType.Create;
        case ChangeType.Update:
          return Enums.ChangeType.Update;
        case ChangeType.Delete:
          return Enums.ChangeType.Delete;
        default:
          throw new NotSupportedException();
      }
    }

    protected override void OnDispose()
    {
      base.OnDispose();

      redisSubscriber.Unsubscribe(ChannelName);
    }

    #endregion
  }
}