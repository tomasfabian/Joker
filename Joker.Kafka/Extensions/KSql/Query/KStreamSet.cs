using System;
using System.Linq.Expressions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  public class KStreamSet<TEntity> : IQbservable<TEntity>
  {
    private readonly Expression expression;

    public KStreamSet(IQbservableProvider provider = null)
    {
      Provider = provider;
      Provider = new QbservableProvider();
      expression = Expression.Constant(this);
    }

    public KStreamSet(IQbservableProvider provider, Expression expression)
    {
      this.Provider = provider;
      this.expression = expression;
    }

    public Expression Expression => expression;

    public Type ElementType => typeof(TEntity);

    public IQbservableProvider Provider { get; }

    public IDisposable Subscribe(IObserver<TEntity> observer)
    {
      var ksqlQuery = new KSqlQueryGenerator().BuildKSql(expression);

      //TODO: implement
      return null;
    }
  }
}