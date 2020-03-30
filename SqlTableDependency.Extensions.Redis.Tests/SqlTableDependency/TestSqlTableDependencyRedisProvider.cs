using System.Reactive.Concurrency;
using Joker.Redis.ConnectionMultiplexers;
using Joker.Redis.SqlTableDependency;
using Joker.Redis.Tests.Models;
using SqlTableDependency.Extensions;

namespace Joker.Redis.Tests.SqlTableDependency
{
  public class TestSqlTableDependencyRedisProvider : SqlTableDependencyRedisProvider<TestModel>
  {
    public TestSqlTableDependencyRedisProvider(ISqlTableDependencyProvider<TestModel> sqlTableDependencyProvider, IRedisPublisher redisPublisher, IScheduler scheduler) 
      : base(sqlTableDependencyProvider, redisPublisher, scheduler)
    {
    }
  }
}