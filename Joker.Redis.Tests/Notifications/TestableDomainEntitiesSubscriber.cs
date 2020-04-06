using System;
using System.Reactive.Linq;
using Joker.Contracts;
using Joker.Factories.Schedulers;
using Joker.Notifications;
using TableDependencyStatuses = Joker.Notifications.VersionedTableDependencyStatus.TableDependencyStatuses;
using Joker.Redis.ConnectionMultiplexers;
using Joker.Redis.Notifications;
using Joker.Redis.Tests.Models;
using Newtonsoft.Json;
using SqlTableDependency.Extensions.Notifications;
using StackExchange.Redis;
using TableDependency.SqlClient.Base.Enums;

namespace Joker.Redis.Tests.Notifications
{
  public class TestableDomainEntitiesSubscriber : DomainEntitiesSubscriber<TestModel>
  {
    public TestableDomainEntitiesSubscriber(IRedisSubscriber redisSubscriber, IEntityChangePublisherWithStatus<TestModel> reactiveData, ISchedulersFactory schedulersFactory) 
      : base(redisSubscriber, reactiveData, schedulersFactory)
    {
    }

    public void PushMessage(ChangeType changeType)
    {
      var notification = new RecordChangedNotification<TestModel>
      {
        Entity = new TestModel(),
        ChangeType = changeType
      };

      var message = JsonConvert.SerializeObject(notification);

      OnMessageReceived(message);
    }

    public void PushStatusMessage(TableDependencyStatuses tableDependencyStatuses)
    {
      var message = CreateStatusJson(tableDependencyStatuses, DateTimeOffset.Now);

      OnStatusMessageReceived(message);
    }

    private static string CreateStatusJson(TableDependencyStatuses tableDependencyStatuses, DateTimeOffset when)
    {
      var status = new VersionedTableDependencyStatus(tableDependencyStatuses, when);

      var message = JsonConvert.SerializeObject(status);

      return message;
    }

    internal override IObservable<string> CreateStatusChangedSource()
    {
      var statusMessagesSource = Observable.Return(CreateStatusJson(TableDependencyStatuses.None, DateTimeOffset.Now.AddSeconds(-5)));

      return base.CreateStatusChangedSource().Merge(statusMessagesSource);
    }
  }
}