using System;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  public abstract class KStreamSet<TEntity> : IQbservable<TEntity>
  {
    private readonly Expression expression;

    protected KStreamSet(IKSqlQbservableProvider provider)
    {
      Provider = provider;
      
      expression = Expression.Constant(this);
    }

    protected KStreamSet(IKSqlQbservableProvider provider, Expression expression)
    {            
      this.Provider = provider;
      this.expression = expression;
    }

    protected abstract IKSqldbProvider<TEntity> CreateKSqlDbProvider();

    protected abstract object CreateQueryParameters(string ksqlQuery);

    public Expression Expression => expression;

    public Type ElementType => typeof(TEntity);

    public IKSqlQbservableProvider Provider { get; }

    public IDisposable Subscribe(IObserver<TEntity> observer)
    {      
      var ksqlQuery = new KSqlQueryGenerator().BuildKSql(expression);

      //TODO: implement
      return Disposable.Empty;
    }
  }
}