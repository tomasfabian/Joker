using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kafka.DotNet.ksqlDB.Infrastructure.Extensions;
using Kafka.DotNet.ksqlDB.KSql.Disposables;
using Kafka.DotNet.ksqlDB.KSql.Linq;
using Kafka.DotNet.ksqlDB.KSql.Query.Options;
using Kafka.DotNet.ksqlDB.KSql.RestApi;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Parameters;
#if !NETSTANDARD
using System.Collections.Generic;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Parameters;
#endif
using Kafka.DotNet.ksqlDB.KSql.RestApi.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kafka.DotNet.ksqlDB.KSql.Query.Context
{
  public class KSqlDBContext : AsyncDisposableObject, IKSqlDBContext
  {
    private readonly KSqlDBContextOptions contextOptions;
    private readonly IServiceCollection serviceCollection;

    public KSqlDBContext(string ksqlDbUrl)
      : this(new KSqlDBContextOptions(ksqlDbUrl))
    {
    }

    public KSqlDBContext(KSqlDBContextOptions contextOptions)
    {
      this.contextOptions = contextOptions ?? throw new ArgumentNullException(nameof(contextOptions));

      serviceCollection = new ServiceCollection();
    }

    private void RegisterDependencies(QueryType queryType)
    {
      OnConfigureServices(serviceCollection);

      serviceCollection.TryAddScoped<IKSqlQbservableProvider, QbservableProvider>();

      var uri = new Uri(contextOptions.Url);

      serviceCollection.TryAddTransient<IKSqlQueryGenerator, KSqlQueryGenerator>();

      if (!serviceCollection.HasRegistration<IHttpClientFactory>())
        serviceCollection.AddSingleton<IHttpClientFactory, HttpClientFactory>(_ =>
          new HttpClientFactory(uri));

      switch (queryType)
      {
        case QueryType.Query:
          serviceCollection.TryAddScoped<IKSqlDbProvider, KSqlDbQueryProvider>();
          serviceCollection.TryAddSingleton(contextOptions.QueryParameters);
          break;
        case QueryType.QueryStream:
          
#if !NETSTANDARD
          serviceCollection.TryAddScoped<IKSqlDbProvider, KSqlDbQueryStreamProvider>();
          serviceCollection.TryAddSingleton<IQueryParameters>(contextOptions.QueryStreamParameters);
          break;
#else
          throw new NotSupportedException();
#endif
      }

      serviceCollection.TryAddSingleton(contextOptions);
      serviceCollection.TryAddScoped<IKStreamSetDependencies, KStreamSetDependencies>();
    }
    
#if !NETSTANDARD
    private ServiceProvider ServiceProvider { get; set; }

    public IAsyncEnumerable<TEntity> CreateQueryStream<TEntity>(QueryStreamParameters queryStreamParameters, CancellationToken cancellationToken = default)
    {
      var serviceScopeFactory = Initialize();

      var ksqlDBProvider = serviceScopeFactory.CreateScope().ServiceProvider.GetService<IKSqlDbProvider>();

      return ksqlDBProvider.Run<TEntity>(queryStreamParameters, cancellationToken);
    }

    public IQbservable<TEntity> CreateQueryStream<TEntity>(string streamName = null)
    {
      var serviceScopeFactory = Initialize();

      if (streamName == String.Empty)
        streamName = null;

      var queryStreamContext = new QueryContext
      {
        StreamName = streamName
      };

      return new KQueryStreamSet<TEntity>(serviceScopeFactory, queryStreamContext);
    }

    private bool wasConfigured;

    internal IServiceScopeFactory Initialize()
    {
      if (!wasConfigured)
      {
        wasConfigured = true;

        RegisterDependencies(QueryType.QueryStream);

        ServiceProvider = serviceCollection.BuildServiceProvider(new ServiceProviderOptions {ValidateScopes = true});
      }

      var serviceScopeFactory = ServiceProvider.GetService<IServiceScopeFactory>();

      return serviceScopeFactory;
    }
#endif

    protected virtual void OnConfigureServices(IServiceCollection serviceCollection)
    {

    }

    private void RegisterHttpClientFactory<TFactory>()
      where TFactory: class, IHttpClientFactory
    {
      serviceCollection.AddSingleton<IHttpClientFactory, TFactory>();
    }

    private void RegisterKSqlDbProvider<TProvider>()
      where TProvider: class, IKSqlDbProvider
    {
      serviceCollection.AddScoped<IKSqlDbProvider, TProvider>();
    }
        
    private ServiceProvider QueriesServiceProvider { get; set; }
    
    public IAsyncEnumerable<TEntity> CreateQuery<TEntity>(QueryParameters queryParameters, CancellationToken cancellationToken = default)
    {
      var serviceScopeFactory = InitializeQueriesServiceProvider();

      var ksqlDBProvider = serviceScopeFactory.CreateScope().ServiceProvider.GetService<IKSqlDbProvider>();

      return ksqlDBProvider.Run<TEntity>(queryParameters, cancellationToken);
    }

    public IQbservable<TEntity> CreateQuery<TEntity>(string streamName = null)
    {
      var serviceScopeFactory = InitializeQueriesServiceProvider();

      if (streamName == String.Empty)
        streamName = null;

      var queryStreamContext = new QueryContext
      {
        StreamName = streamName
      };

      return new KQueryStreamSet<TEntity>(serviceScopeFactory, queryStreamContext);
    }
    
    private bool wereQueriesConfigured;

    internal IServiceScopeFactory InitializeQueriesServiceProvider()
    {
      if (!wereQueriesConfigured)
      {
        wereQueriesConfigured = true;

        RegisterDependencies(QueryType.Query);

        QueriesServiceProvider = serviceCollection.BuildServiceProvider(new ServiceProviderOptions {ValidateScopes = true});
      }

      var serviceScopeFactory = QueriesServiceProvider.GetService<IServiceScopeFactory>();

      return serviceScopeFactory;
    }

    protected override async ValueTask OnDisposeAsync()
    {
#if !NETSTANDARD
      if(ServiceProvider != null)
        await ServiceProvider.DisposeAsync();
#endif
      if(QueriesServiceProvider != null)
        await QueriesServiceProvider.DisposeAsync();
    }
  }
}