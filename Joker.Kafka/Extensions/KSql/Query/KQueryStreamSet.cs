using System.Linq.Expressions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  public class KQueryStreamSet<TEntity> : KStreamSet<TEntity>
  {
    public KQueryStreamSet(IServiceScopeFactory serviceScopeFactory, QueryContext queryContext) 
      : base(serviceScopeFactory, queryContext)
    {
    }

    public KQueryStreamSet(IServiceScopeFactory serviceScopeFactory, QueryContext queryContext, Expression expression) 
      : base(serviceScopeFactory, expression, queryContext)
    {
    }
  }
}