using System;
using System.Linq.Expressions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  internal class KQuerySet<TEntity> : KStreamSet<TEntity>
  {
    public KQuerySet(IServiceScopeFactory serviceScopeFactory, QueryContext queryContext)
      : base(serviceScopeFactory, queryContext)
    {
    }

    public KQuerySet(IServiceScopeFactory serviceScopeFactory, QueryContext queryContext, Expression expression)
      : base(serviceScopeFactory, expression, queryContext)
    {
    }
  }
}