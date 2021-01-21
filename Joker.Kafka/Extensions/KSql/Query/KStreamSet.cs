using System;
using System.Linq.Expressions;
using System.Threading;
using System.Linq;
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

      var ksqlDBProvider = CreateKSqlDbProvider();

      var cancellationTokenSource = new CancellationTokenSource();

      var queryParameters = CreateQueryParameters(ksqlQuery);
      
      var querySubscription = ksqlDBProvider.Run(queryParameters, cancellationTokenSource.Token)
        .ToObservable()
        .Subscribe(observer);
      
      var compositeDisposable = new CompositeDisposable
      {
        Disposable.Create(() => cancellationTokenSource.Cancel()), 
        querySubscription, 
        cancellationTokenSource
      };

      return compositeDisposable;
    }
  }
}