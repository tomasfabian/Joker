using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  public interface IKStreamSetDependencies
  {
    IKSqlQbservableProvider Provider { get; }
    IKSqldbProvider KsqlDBProvider { get; }
  }
}