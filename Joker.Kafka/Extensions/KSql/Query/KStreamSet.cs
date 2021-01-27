using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Linq;
using System.Reactive.Disposables;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query.Context;

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

    public Expression Expression { get; }

    public Type ElementType => typeof(TEntity);

    public IKSqlQbservableProvider Provider { get; }

    internal IKSqlQueryGenerator KSqlQueryGenerator => dependencies.KSqlQueryGenerator;

    internal QueryContext QueryContext => dependencies.QueryContext;

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

    internal IAsyncEnumerable<TEntity> RunStreamAsAsyncEnumerable(CancellationTokenSource cancellationTokenSource = default)
    {
      var cancellationToken = cancellationTokenSource?.Token ?? CancellationToken.None;

      var ksqlQuery = dependencies.KSqlQueryGenerator.BuildKSql(Expression, dependencies.QueryContext);

      var ksqlDBProvider = dependencies.KsqlDBProvider;

      var queryParameters = dependencies.QueryStreamParameters;
      queryParameters.Sql = ksqlQuery;

      return ksqlDBProvider.Run<TEntity>(queryParameters, cancellationToken);
    }

    internal IObservable<TEntity> RunStreamAsObservable(CancellationTokenSource cancellationTokenSource = default)
    {
      var observableStream = RunStreamAsAsyncEnumerable(cancellationTokenSource)
        .ToObservable();

      return observableStream;
    }
  }
}