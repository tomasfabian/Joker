using System.Linq.Expressions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;

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
  }
}