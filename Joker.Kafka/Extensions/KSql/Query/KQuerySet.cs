using System;
using System.Linq.Expressions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Parameters;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  internal class KQuerySet<TEntity> : KStreamSet<TEntity>
  {
    internal KQuerySet(IKSqlQbservableProvider provider) 
      : base(provider)
    {
    }

    internal KQuerySet(IKSqlQbservableProvider provider, Expression expression) 
      : base(provider, expression)
    {
    }
    
    protected override IKSqldbProvider<TEntity> CreateKSqlDbProvider()
    {
      var uri = new Uri(Provider.Url);

      var httpClientFactory = new HttpClientFactory(uri);

      return new KSqlDbQueryProvider<TEntity>(httpClientFactory);
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