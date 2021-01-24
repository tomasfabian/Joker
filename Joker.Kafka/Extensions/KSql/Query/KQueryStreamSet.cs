using System.Linq.Expressions;

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
  }
}