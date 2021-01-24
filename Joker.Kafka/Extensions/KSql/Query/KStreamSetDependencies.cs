using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Parameters;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  public class KStreamSetDependencies : IKStreamSetDependencies
  {
    public KStreamSetDependencies(IKSqlQbservableProvider provider, IKSqldbProvider ksqlDBProvider, IKSqlQueryGenerator queryGenerator, QueryStreamParameters queryStreamParameters)
    {
      Provider = provider;
      KsqlDBProvider = ksqlDBProvider;
      KSqlQueryGenerator = queryGenerator;
      QueryStreamParameters = queryStreamParameters;
    }

    public IKSqlQbservableProvider Provider { get; }

    public IKSqldbProvider KsqlDBProvider { get; }
    public IKSqlQueryGenerator KSqlQueryGenerator { get; }

    public QueryStreamParameters QueryStreamParameters { get; }
  }
}