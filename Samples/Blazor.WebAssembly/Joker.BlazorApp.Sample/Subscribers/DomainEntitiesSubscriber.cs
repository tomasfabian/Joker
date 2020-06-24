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
using Sample.Domain.Models;
using SqlTableDependency.Extensions.Notifications;
using ChangeType = TableDependency.SqlClient.Base.Enums.ChangeType;

namespace Joker.BlazorApp.Sample.Subscribers
{
  public interface IDomainEntitiesSubscriber
  {
    Task Subscribe();
  }
  
  public class DomainEntitiesSubscriber<TEntity> : DisposableObject, IDomainEntitiesSubscriber, ITableDependencyStatusProvider 
    where TEntity : IVersion
  {
    private readonly NavigationManager navigationManager;
    private readonly IEntityChangePublisherWithStatus<TEntity> reactiveData;
    private readonly ISchedulersFactory schedulersFactory;

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

    protected virtual string ChannelName { get; } = "ReceiveDataChange";

    protected virtual string StatusChannelName { get; } = typeof(TEntity).Name + "-Status";

    #region Methods

    private HubConnection hubConnection;

    public async Task Subscribe()
    {
      if (IsDisposed)
        throw new ObjectDisposedException("Object has already been disposed.");

      hubConnection = new HubConnectionBuilder()
        .WithAutomaticReconnect()
        .WithUrl(navigationManager.ToAbsoluteUri("/dataChangesHub"))
        .Build();

      hubConnection.Closed += exception =>
      {
        Console.WriteLine($"Closed: {exception}");
        return Task.CompletedTask;
      };
      hubConnection.Reconnected += s =>
      {
        Console.WriteLine($"Reconnected: {s}");

        return Task.CompletedTask;
      };
      hubConnection.Reconnecting += exception =>
      {
        Console.WriteLine($"Reconnecting: {exception}");
        
        return Task.CompletedTask;
      };
      hubConnection.On<RecordChangedNotification<TEntity>>(ChannelName, recordChangedNotification =>
      {
        Console.WriteLine($"Received notification: {recordChangedNotification}");

        OnMessageReceived(recordChangedNotification);
      });
      
      //TODO: remove when status provider will be ready
      ReactiveDataWithStatus<Product>.Instance.Publish(new VersionedTableDependencyStatus(VersionedTableDependencyStatus.TableDependencyStatuses.Started, DateTimeOffset.Now));
      
      await hubConnection.StartAsync();
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

    public IObservable<VersionedTableDependencyStatus> WhenStatusChanges { get; } =
      ReactiveDataWithStatus<Product>.Instance.WhenStatusChanges;
  }
}