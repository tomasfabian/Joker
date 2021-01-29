using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Linq;
using System.Reactive.Disposables;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  internal abstract class KStreamSet<TEntity> : IQbservable<TEntity>
  {
    private readonly IKStreamSetDependencies dependencies;
    private readonly IServiceScope serviceScope;

    protected KStreamSet(IServiceScopeFactory serviceScopeFactory, QueryContext queryContext = null)
    {
      QueryContext = queryContext;

      serviceScope = serviceScopeFactory.CreateScope();
      
      dependencies = serviceScope.ServiceProvider.GetService<IKStreamSetDependencies>();

      Provider = new QbservableProvider(serviceScopeFactory, queryContext);
      
      Expression = Expression.Constant(this);
    }

    protected KStreamSet(IServiceScopeFactory serviceScopeFactory, Expression expression, QueryContext queryContext = null)
    {
      QueryContext = queryContext;

      serviceScope = serviceScopeFactory.CreateScope();
      
      dependencies = serviceScope.ServiceProvider.GetService<IKStreamSetDependencies>();

      Provider = new QbservableProvider(serviceScopeFactory, queryContext);

      Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    public Expression Expression { get; }

    public Type ElementType => typeof(TEntity);

    public IKSqlQbservableProvider Provider { get; }

    internal IKSqlQueryGenerator KSqlQueryGenerator => dependencies.KSqlQueryGenerator;

    internal QueryContext QueryContext { get; }

    internal IServiceScope ServiceScope => serviceScope;

    public IDisposable Subscribe(IObserver<TEntity> observer)
    {
      var cancellationTokenSource = new CancellationTokenSource(); 

      var querySubscription = RunStreamAsObservable(cancellationTokenSource)
        .Subscribe(observer);

      var compositeDisposable = new CompositeDisposable
      {
        Disposable.Create(() => cancellationTokenSource.Cancel()), 
        querySubscription
      };

      return compositeDisposable;
    }

    internal IAsyncEnumerable<TEntity> RunStreamAsAsyncEnumerable(CancellationTokenSource cancellationTokenSource)
    {
      var cancellationToken = cancellationTokenSource.Token;

      cancellationToken.Register(() => serviceScope.Dispose());

      var ksqlQuery = dependencies.KSqlQueryGenerator.BuildKSql(Expression, QueryContext);

      var ksqlDBProvider = dependencies.KsqlDBProvider;

      var queryParameters = dependencies.QueryStreamParameters;
      queryParameters.Sql = ksqlQuery;

      return ksqlDBProvider.Run<TEntity>(queryParameters, cancellationToken);
    }

    internal IObservable<TEntity> RunStreamAsObservable(CancellationTokenSource cancellationTokenSource)
    {
      var observableStream = RunStreamAsAsyncEnumerable(cancellationTokenSource)
        .ToObservable();

      return observableStream;
    }
  }
}