using System.Linq.Expressions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Parameters;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  public class KQueryStreamSet<TEntity> : KStreamSet<TEntity>
  {
    public KQueryStreamSet(IKStreamSetDependencies dependencies) 
      : base(dependencies)
    {
    }

    public KQueryStreamSet(IKStreamSetDependencies dependencies, Expression expression) 
      : base(dependencies, expression)
    {
    }

#if NETCOREAPP3_1
    protected override object CreateQueryParameters(string ksqlQuery)
#else
    protected override QueryStreamParameters CreateQueryParameters(string ksqlQuery)
#endif
    {
      var queryParameters = new QueryStreamParameters
      {
        Sql = ksqlQuery,
        ["auto.offset.reset"] = "earliest"
      };

      return queryParameters;
    }
  }
}