using SqlTableDependency.Extensions.Redis.ConnectionMultiplexers;
using SqlTableDependency.Extensions.Redis.SqlTableDependency;
using SqlTableDependency.Extensions.Tests.Models;

namespace SqlTableDependency.Extensions.Redis.Tests.SqlTableDependency
{
  public class TestClass : SqlTableDependencyRedisProvider<TestModel>
  {
    public TestClass(ISqlTableDependencyProvider<TestModel> sqlTableDependencyProvider, IRedisPublisher redisPublisher) 
      : base(sqlTableDependencyProvider, redisPublisher)
    {
    }
  }
}