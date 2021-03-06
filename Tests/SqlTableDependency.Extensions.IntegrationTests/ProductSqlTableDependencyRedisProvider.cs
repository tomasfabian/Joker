using System.Reactive.Concurrency;
using Joker.Redis.ConnectionMultiplexers;
using Joker.Redis.SqlTableDependency;
using Sample.Domain.Models;

namespace SqlTableDependency.Extensions.IntegrationTests
{
  public class ProductSqlTableDependencyRedisProvider : SqlTableDependencyRedisProvider<Product>
  {
    public ProductSqlTableDependencyRedisProvider(ISqlTableDependencyProvider<Product> sqlTableDependencyProvider, IRedisPublisher redisPublisher, IScheduler scheduler) 
      : base(sqlTableDependencyProvider, redisPublisher, scheduler)
    {
    }
  }
}