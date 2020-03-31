using Joker.Contracts;
using Joker.Redis.ConnectionMultiplexers;
using Joker.Redis.Notifications;
using Joker.Redis.Tests.Models;
using Newtonsoft.Json;
using SqlTableDependency.Extensions.Notifications;
using TableDependency.SqlClient.Base.Enums;

namespace Joker.Redis.Tests.Notifications
{
  public class TestableDomainEntitiesSubscriber : DomainEntitiesSubscriber<TestModel>
  {
    public TestableDomainEntitiesSubscriber(IRedisSubscriber redisSubscriber, IPublisher<TestModel> reactiveData) 
      : base(redisSubscriber, reactiveData)
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
  }
}