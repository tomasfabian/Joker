using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Parameters;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  public interface IKStreamSetDependencies
  {
    IKSqlQbservableProvider Provider { get; }
    IKSqldbProvider KsqlDBProvider { get; }
    IKSqlQueryGenerator KSqlQueryGenerator { get; }
    QueryStreamParameters QueryStreamParameters { get; }
    QueryContext QueryContext { get; }
  }
}