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
    protected KStreamSet(IKSqlQbservableProvider provider)
    {
      Provider = provider;
      
      Expression = Expression.Constant(this);
    }

    protected KStreamSet(IKSqlQbservableProvider provider, Expression expression)
    {            
      Provider = provider;
      Expression = expression;
    }

    protected abstract IKSqldbProvider<TEntity> CreateKSqlDbProvider();

    public virtual IKSqlQueryGenerator CreateKSqlQueryGenerator()
    {
      return new KSqlQueryGenerator();
    }

    protected abstract object CreateQueryParameters(string ksqlQuery);

    public Expression Expression { get; }

    public Type ElementType => typeof(TEntity);

    public IKSqlQbservableProvider Provider { get; }

    public IDisposable Subscribe(IObserver<TEntity> observer)
    {
      var cancellationTokenSource = new CancellationTokenSource(); 

      var querySubscription = RunStreamAsObservable(cancellationTokenSource)
        .Subscribe(observer);

      var compositeDisposable = new CompositeDisposable
      {
        Disposable.Create(() => cancellationTokenSource.Cancel()), 
        querySubscription, 
        cancellationTokenSource
      };

      return compositeDisposable;
    }

    internal IObservable<TEntity> RunStreamAsObservable(CancellationTokenSource cancellationTokenSource = default)
    {
      var ksqlQuery = CreateKSqlQueryGenerator().BuildKSql(Expression);

      var ksqlDBProvider = CreateKSqlDbProvider();

      var queryParameters = CreateQueryParameters(ksqlQuery);

      var observableStream = ksqlDBProvider.Run(queryParameters, cancellationTokenSource.Token)
        .ToObservable();

      return observableStream;
    }
  }
}