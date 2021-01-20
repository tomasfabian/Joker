using System.Linq.Expressions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;

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
  }
}