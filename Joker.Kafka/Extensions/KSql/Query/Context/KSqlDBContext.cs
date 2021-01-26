using System;
using System.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query.Context
{
  public class KSqlDBContext : IKSqlDBContext
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

    private void RegisterDependencies()
    {
      OnConfigureServices(serviceCollection);

      serviceCollection.TryAddTransient<IKSqlQbservableProvider, QbservableProvider>();

      var uri = new Uri(contextOptions.Url);

      serviceCollection.TryAddTransient<IKSqlQueryGenerator, KSqlQueryGenerator>();

      if (!HasRegistration<IHttpClientFactory>())
        serviceCollection.AddSingleton<IHttpClientFactory, HttpClientFactory>(_ =>
          new HttpClientFactory(uri));
      
      serviceCollection.TryAddTransient<IKSqlDbProvider, KSqlDbQueryStreamProvider>();
      serviceCollection.TryAddSingleton(contextOptions);
      serviceCollection.TryAddSingleton(contextOptions.QueryStreamParameters);
      serviceCollection.TryAddTransient<IKStreamSetDependencies, KStreamSetDependencies>();
    }

    protected virtual void OnConfigureServices(IServiceCollection serviceCollection)
    {

    }

    private bool HasRegistration<TType>()
    {
      return serviceCollection.Any(x => x.ServiceType == typeof(TType));
    }

    private void RegisterHttpClientFactory<TFactory>()
      where TFactory: class, IHttpClientFactory
    {
      serviceCollection.AddSingleton<IHttpClientFactory, TFactory>();
    }

    private void RegisterKSqlDbProvider<TProvider>()
      where TProvider: class, IKSqlDbProvider
    {
      serviceCollection.AddTransient<IKSqlDbProvider, TProvider>();
    }

    private bool wasConfigured;

    public IQbservable<TEntity> CreateStreamSet<TEntity>(string streamName = null)
    {
      if(!wasConfigured)
      {
        wasConfigured = true;

        RegisterDependencies();
      }

      if (streamName == String.Empty)
        streamName = null;

      var serviceProvider = serviceCollection.BuildServiceProvider(new ServiceProviderOptions {ValidateScopes = true});

      var dependencies = serviceProvider.GetService<IKStreamSetDependencies>();
      
      dependencies.QueryContext.StreamName = streamName;

      return new KQueryStreamSet<TEntity>(dependencies);
    }
  }
}