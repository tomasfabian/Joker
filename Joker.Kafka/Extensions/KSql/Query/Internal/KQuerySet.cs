using System;
using System.Linq.Expressions;

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
  }
}