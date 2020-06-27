using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Joker.Contracts;
using Joker.Disposables;
using Joker.Enums;
using Joker.Extensions.Disposables;
using Joker.Factories.Schedulers;
using Joker.Notifications;
using Joker.Reactive;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Sample.Domain.Models;
using SqlTableDependency.Extensions.Notifications;
using ChangeType = TableDependency.SqlClient.Base.Enums.ChangeType;

namespace Joker.BlazorApp.Sample.Subscribers
{
  public interface IDomainEntitiesSubscriber
  {
    Task Subscribe();
  }

  public class DomainEntitiesSubscriber<TEntity> : DisposableObject, IDomainEntitiesSubscriber//, ITableDependencyStatusProvider 
    where TEntity : IVersion
  {
    #region Fields

    private readonly NavigationManager navigationManager;
    private readonly IEntityChangePublisherWithStatus<TEntity> reactiveData;
    private readonly ISchedulersFactory schedulersFactory;

    #endregion

    #region Constructors

    public DomainEntitiesSubscriber(
      NavigationManager navigationManager,
      IEntityChangePublisherWithStatus<TEntity> reactiveData,
      ISchedulersFactory schedulersFactory)
    {
      this.navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
      this.reactiveData = reactiveData ?? throw new ArgumentNullException(nameof(reactiveData));
      this.schedulersFactory = schedulersFactory ?? throw new ArgumentNullException(nameof(schedulersFactory));
      
      statusChangesSubscription = new SerialDisposable();
      statusChangesSubscription.DisposeWith(CompositeDisposable);
    }

    #endregion

    //public bool IsConnected =>
    //  hubConnection?.State == HubConnectionState.Connected;

    protected virtual string ChannelName { get; } = "ReceiveDataChange";

    protected virtual string StatusChannelName { get; } = typeof(TEntity).Name + "-Status";

    #region Methods

    private HubConnection hubConnection;

    public async Task Subscribe()
    {
      if (IsDisposed)
        throw new ObjectDisposedException("Object has already been disposed.");

      hubConnection = new HubConnectionBuilder()
        .WithAutomaticReconnect(new SignalRRetryPolicy())
        .WithUrl(navigationManager.ToAbsoluteUri("/dataChangesHub"))
        .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Error))
        .Build();

      SubscribeToHubEvents();

      hubConnection.On<RecordChangedNotification<TEntity>>(ChannelName, recordChangedNotification =>
      {
        Console.WriteLine($"Received notification: {recordChangedNotification}");

        OnMessageReceived(recordChangedNotification);
      });
      
      await hubConnection.StartAsync().ContinueWith(c =>
      {
        Console.WriteLine("Started: " + c.Status);
        //TODO: remove when status provider will be ready
        if(c.Status == TaskStatus.RanToCompletion)
          PublishStatusChanged(VersionedTableDependencyStatus.TableDependencyStatuses.Started);
      });
    }

    private void SubscribeToHubEvents()
    {
      hubConnection.Closed += exception =>
      {
        Console.WriteLine("Closed: " + exception);
        //TODO: remove when status provider will be ready
        PublishStatusChanged(VersionedTableDependencyStatus.TableDependencyStatuses.StopDueToError);

        return Task.CompletedTask;
      };

      hubConnection.Reconnected += s =>
      {
        Console.WriteLine("Reconnected: " + s);
        //TODO: remove when status provider will be ready
        PublishStatusChanged(VersionedTableDependencyStatus.TableDependencyStatuses.Started);

        return Task.CompletedTask;
      };

      hubConnection.Reconnecting += exception =>
      {
        Console.WriteLine("Reconnecting: " + exception);
        //TODO: remove when status provider will be ready
        PublishStatusChanged(VersionedTableDependencyStatus.TableDependencyStatuses.StopDueToError);

        return Task.CompletedTask;
      };
    }

    private void PublishStatusChanged(VersionedTableDependencyStatus.TableDependencyStatuses status)
    {
      reactiveData.Publish(new VersionedTableDependencyStatus(
        status, DateTimeOffset.Now));
    }

    private VersionedTableDependencyStatus lastStatus;

    private void SubscribeToConnectionChanged()
    {
      //TODO:
    }

    private readonly SerialDisposable statusChangesSubscription;

    private void SubscribeToStatusChanges()
    {
      //TODO:
    }

    internal void OnMessageReceived(RecordChangedNotification<TEntity> recordChange)
    {
      if (recordChange.ChangeType == ChangeType.None) 
        return;

      var entityChange = new EntityChange<TEntity>(recordChange.Entity, Convert(recordChange.ChangeType));

      reactiveData.Publish(entityChange);
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

      _ = hubConnection.DisposeAsync();
    }

    #endregion

    //public IObservable<VersionedTableDependencyStatus> WhenStatusChanges { get; } =
    //  ReactiveDataWithStatus<Product>.Instance.WhenStatusChanges;
  }
}