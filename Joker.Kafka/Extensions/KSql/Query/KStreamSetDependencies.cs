using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Parameters;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  public class KStreamSetDependencies : IKStreamSetDependencies
  {
    public KStreamSetDependencies(IKSqlQbservableProvider provider, IKSqldbProvider ksqlDBProvider, QueryStreamParameters queryStreamParameters)
    {
      Provider = provider;
      KsqlDBProvider = ksqlDBProvider;
      QueryStreamParameters = queryStreamParameters;
    }

    public IKSqlQbservableProvider Provider { get; }

    public IKSqldbProvider KsqlDBProvider { get; }
    public QueryStreamParameters QueryStreamParameters { get; }
  }
}