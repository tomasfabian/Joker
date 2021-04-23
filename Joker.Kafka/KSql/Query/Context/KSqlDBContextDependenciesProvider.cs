﻿using System;
using System.Threading.Tasks;
using Kafka.DotNet.ksqlDB.Infrastructure.Extensions;
using Kafka.DotNet.ksqlDB.KSql.Disposables;
using Kafka.DotNet.ksqlDB.KSql.Linq;
using Kafka.DotNet.ksqlDB.KSql.RestApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kafka.DotNet.ksqlDB.KSql.Query.Context
{
  public abstract class KSqlDBContextDependenciesProvider : AsyncDisposableObject
  {
    private readonly IServiceCollection serviceCollection;

    protected KSqlDBContextDependenciesProvider()
    {
      serviceCollection = new ServiceCollection();
    }

    protected ServiceProvider ServiceProvider { get; set; }
    
    private bool wasConfigured;

    internal IServiceScopeFactory Initialize(KSqlDBContextOptions contextOptions)
    {
      if (!wasConfigured)
      {
        wasConfigured = true;

        RegisterDependencies(contextOptions);

        ServiceProvider = serviceCollection.BuildServiceProvider(new ServiceProviderOptions {ValidateScopes = true});
      }

      var serviceScopeFactory = ServiceProvider.GetService<IServiceScopeFactory>();

      return serviceScopeFactory;
    }

    private void RegisterDependencies(KSqlDBContextOptions contextOptions)
    {
      OnConfigureServices(serviceCollection, contextOptions);
    }

    protected virtual void OnConfigureServices(IServiceCollection serviceCollection, KSqlDBContextOptions contextOptions)
    {
      serviceCollection.TryAddScoped<IKSqlQbservableProvider, QbservableProvider>();

      var uri = new Uri(contextOptions.Url);

      serviceCollection.TryAddTransient<IKSqlQueryGenerator, KSqlQueryGenerator>();

      if (!serviceCollection.HasRegistration<IHttpClientFactory>())
        serviceCollection.AddSingleton<IHttpClientFactory, HttpClientFactory>(_ =>
          new HttpClientFactory(uri));

      serviceCollection.TryAddSingleton(contextOptions);
      serviceCollection.TryAddScoped<IKStreamSetDependencies, KStreamSetDependencies>();
      serviceCollection.TryAddScoped<IKSqlDbRestApiClient, KSqlDbRestApiClient>();
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
    
    protected override async ValueTask OnDisposeAsync()
    {
      if(ServiceProvider != null)
        await ServiceProvider.DisposeAsync();
    }
  }
}