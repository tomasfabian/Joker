using System;
using System.Linq.Expressions;
using System.Threading;
using System.Linq;
using System.Reactive.Disposables;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  public abstract class KStreamSet<TEntity> : IQbservable<TEntity>
  {
    private readonly IKStreamSetDependencies dependencies;

    protected KStreamSet(IKStreamSetDependencies dependencies)
    {
      this.dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));

      Provider = dependencies.Provider;
      Expression = Expression.Constant(this);
    }

    protected KStreamSet(IKStreamSetDependencies dependencies, Expression expression)
    {
      this.dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));

      Provider = dependencies.Provider;
      Expression = expression;
    }

    public virtual IKSqlQueryGenerator CreateKSqlQueryGenerator()
    {
      return new KSqlQueryGenerator();
    }

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

      var ksqlDBProvider = dependencies.KsqlDBProvider;


      var queryParameters = dependencies.QueryStreamParameters;
      queryParameters.Sql = ksqlQuery;

      var observableStream = ksqlDBProvider.Run<TEntity>(queryParameters, cancellationTokenSource.Token)
        .ToObservable();

      return observableStream;
    }
  }
}