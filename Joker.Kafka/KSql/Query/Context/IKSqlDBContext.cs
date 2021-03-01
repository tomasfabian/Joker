using System;
using Kafka.DotNet.ksqlDB.KSql.Linq;

namespace Kafka.DotNet.ksqlDB.KSql.Query.Context
{
  public interface IKSqlDBContext : IAsyncDisposable
  {
#if !NETSTANDARD
    IQbservable<TEntity> CreateQueryStream<TEntity>(string streamName = null);
#endif

    IQbservable<TEntity> CreateQuery<TEntity>(string streamName = null);
  }
}