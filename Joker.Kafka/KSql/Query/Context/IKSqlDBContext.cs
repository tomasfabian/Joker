using System;
using Kafka.DotNet.ksqlDB.KSql.Linq;

namespace Kafka.DotNet.ksqlDB.KSql.Query.Context
{
  public interface IKSqlDBContext : IAsyncDisposable
  {
    IQbservable<TEntity> CreateQueryStream<TEntity>(string streamName = null);
  }
}