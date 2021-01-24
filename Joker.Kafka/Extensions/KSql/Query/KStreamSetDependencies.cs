using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  public class KStreamSetDependencies : IKStreamSetDependencies
  {
    public KStreamSetDependencies(IKSqlQbservableProvider provider, IKSqldbProvider ksqlDBProvider)
    {
      Provider = provider;
      KsqlDBProvider = ksqlDBProvider;
    }

    public IKSqlQbservableProvider Provider { get; }

    public IKSqldbProvider KsqlDBProvider { get; }
  }
}