using System.Linq.Expressions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Parameters;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  public class KQuerySet<TEntity> : KStreamSet<TEntity>
  {
    public KQuerySet(IKSqlQbservableProvider provider) 
      : base(provider)
    {
    }

    public KQuerySet(IKSqlQbservableProvider provider, Expression expression) 
      : base(provider, expression)
    {
    }
    
    protected override IKSqldbProvider<TEntity> CreateKSqlDbProvider()
    {
      return null;
    }

#if NETCOREAPP3_1
    protected override object CreateQueryParameters(string ksqlQuery)
#else
    protected override KsqlQueryParameters CreateQueryParameters(string ksqlQuery)
#endif
    {
      var queryParameters = new KsqlQueryParameters
      {
        KSql = ksqlQuery,
        ["ksql.streams.auto.offset.reset"] = "earliest"
      };

      return queryParameters;
    }
  }
}