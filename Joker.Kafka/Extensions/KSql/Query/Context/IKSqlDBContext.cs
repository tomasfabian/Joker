using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query.Context
{
  public interface IKSqlDBContext
  {
    IQbservable<TEntity> CreateStreamSet<TEntity>(string streamName = null);
  }
}