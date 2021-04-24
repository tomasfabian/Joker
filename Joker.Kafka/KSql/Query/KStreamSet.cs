using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Threading;
using Kafka.DotNet.ksqlDB.KSql.Linq;
using Kafka.DotNet.ksqlDB.KSql.Query.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Kafka.DotNet.ksqlDB.KSql.Query
{
  internal abstract class KStreamSet : IQbservable
  {
    public abstract Type ElementType { get; }
    public Expression Expression { get; internal set; }
    public IKSqlQbservableProvider Provider { get; internal set; }
    
    internal QueryContext QueryContext { get; set; }
  }

  internal abstract class KStreamSet<TEntity> : KStreamSet, IQbservable<TEntity>
  {
    private readonly IServiceScopeFactory serviceScopeFactory;
    private IServiceScope serviceScope;

    protected KStreamSet(IServiceScopeFactory serviceScopeFactory, QueryContext queryContext = null)
    {
      this.serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
      
      QueryContext = queryContext;

      Provider = new QbservableProvider(serviceScopeFactory, queryContext);
      
      Expression = Expression.Constant(this);
    }

    protected KStreamSet(IServiceScopeFactory serviceScopeFactory, Expression expression, QueryContext queryContext = null)
    {
      this.serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
      
      QueryContext = queryContext;

      Provider = new QbservableProvider(serviceScopeFactory, queryContext);

      Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    public override Type ElementType => typeof(TEntity);

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
      
      serviceScope = serviceScopeFactory.CreateScope();
      var dependencies = serviceScope.ServiceProvider.GetService<IKStreamSetDependencies>();

      cancellationToken.Register(() => serviceScope?.Dispose());

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

    internal string BuildKsql()
    {
      serviceScope = serviceScopeFactory.CreateScope();
      
      var dependencies = serviceScope.ServiceProvider.GetService<IKStreamSetDependencies>();

      var ksqlQuery = dependencies.KSqlQueryGenerator?.BuildKSql(Expression, QueryContext);
      
      serviceScope.Dispose();

      return ksqlQuery;
    }
  }
}