using System;
using System.Threading.Tasks;
using Joker.Contracts;
using Joker.Disposables;
using Joker.Enums;
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
    private readonly IPublisher<TEntity> reactiveData;

    public DomainEntitiesSubscriber(IRedisSubscriber redisSubscriber, IPublisher<TEntity> reactiveData)
    {
      this.redisSubscriber = redisSubscriber ?? throw new ArgumentNullException(nameof(redisSubscriber));
      this.reactiveData = reactiveData ?? throw new ArgumentNullException(nameof(reactiveData));
    }

    protected virtual string ChannelName { get; } = typeof(TEntity).Name + "-Changes";

    #region Methods

    public async Task Subscribe()
    {
      if (IsDisposed)
        throw new ObjectDisposedException("Object has already been disposed.");

      await redisSubscriber.Subscribe(OnMessageReceived, ChannelName);
    }

    private void OnMessageReceived(ChannelMessage channelMessage)
    {
      OnMessageReceived(channelMessage.Message);
    }

    protected void OnMessageReceived(string message)
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