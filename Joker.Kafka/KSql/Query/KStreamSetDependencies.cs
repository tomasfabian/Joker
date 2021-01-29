using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query.Context;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Parameters;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  internal class KStreamSetDependencies : IKStreamSetDependencies
  {
    public KStreamSetDependencies(IKSqlQbservableProvider provider, IKSqlDbProvider ksqlDBProvider, IKSqlQueryGenerator queryGenerator, QueryStreamParameters queryStreamParameters)
    {
      Provider = provider;
      KsqlDBProvider = ksqlDBProvider;
      KSqlQueryGenerator = queryGenerator;
      QueryStreamParameters = queryStreamParameters;

      QueryContext = new QueryContext();
    }

    public IKSqlQbservableProvider Provider { get; }

    public IKSqlDbProvider KsqlDBProvider { get; }

    public IKSqlQueryGenerator KSqlQueryGenerator { get; }

    public QueryStreamParameters QueryStreamParameters { get; }

    public QueryContext QueryContext { get; }
  }
}