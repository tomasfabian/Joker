using System;
using System.Linq.Expressions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Parameters;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  internal class KQuerySet<TEntity> : KStreamSet<TEntity>
  {
    internal KQuerySet(IKStreamSetDependencies dependencies)
      : base(dependencies)
    {
    }

    internal KQuerySet(IKStreamSetDependencies dependencies, Expression expression)
      : base(dependencies, expression)
    {
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