using System;
using System.Linq.Expressions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Parameters;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  public class KQueryStreamSet<TEntity> : KStreamSet<TEntity>
  {
    public KQueryStreamSet(IKSqlQbservableProvider provider) 
      : base(provider)
    {
    }

    public KQueryStreamSet(IKSqlQbservableProvider provider, Expression expression) 
      : base(provider, expression)
    {
    }
    
    protected override IKSqldbProvider<TEntity> CreateKSqlDbProvider()
    {
      var uri = new Uri(Provider.Url);

      var httpClientFactory = new HttpClientFactory(uri);
      
      return new KSqlDbQueryStreamProvider<TEntity>(httpClientFactory);
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