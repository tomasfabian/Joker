using System.Reactive.Concurrency;
using SqlTableDependency.Extensions.Redis.ConnectionMultiplexers;
using SqlTableDependency.Extensions.Redis.SqlTableDependency;
using SqlTableDependency.Extensions.Tests.Models;

namespace SqlTableDependency.Extensions.Redis.Tests.SqlTableDependency
{
  public class TestSqlTableDependencyRedisProvider : SqlTableDependencyRedisProvider<TestModel>
  {
    public TestSqlTableDependencyRedisProvider(ISqlTableDependencyProvider<TestModel> sqlTableDependencyProvider, IRedisPublisher redisPublisher, IScheduler scheduler) 
      : base(sqlTableDependencyProvider, redisPublisher, scheduler)
    {
    }
  }
}