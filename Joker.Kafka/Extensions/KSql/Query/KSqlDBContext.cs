using System;
using System.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  public sealed class KSqlDBContext : IKSqlDBContext
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
      serviceCollection.TryAddTransient<IKSqlQbservableProvider, QbservableProvider>();

      var uri = new Uri(contextOptions.Url);

      serviceCollection.TryAddTransient<IKSqlQueryGenerator, KSqlQueryGenerator>();

      if(!HasRegistration<IHttpClientFactory>())
        serviceCollection.AddSingleton<IHttpClientFactory, HttpClientFactory>(_ =>
          new HttpClientFactory(uri));
      
      serviceCollection.TryAddTransient<IKSqldbProvider, KSqlDbQueryStreamProvider>();
      serviceCollection.TryAddSingleton(contextOptions);
      serviceCollection.TryAddSingleton(contextOptions.QueryStreamParameters);
      serviceCollection.TryAddTransient<IKStreamSetDependencies, KStreamSetDependencies>();
    }

    private bool HasRegistration<TType>()
    {
      return serviceCollection.Any(x => x.ServiceType == typeof(TType));
    }

    public void RegisterHttpClientFactory<TFactory>()
      where TFactory: class, IHttpClientFactory
    {
      serviceCollection.AddSingleton<IHttpClientFactory, TFactory>();
    }

    public void RegisterKSqlDbProvider<KSqlDbProvider>()
      where KSqlDbProvider: class, IKSqldbProvider
    {
      serviceCollection.AddTransient<IKSqldbProvider, KSqlDbProvider>();
    }

    private bool wasRegistered;

    public IQbservable<TEntity> CreateStreamSet<TEntity>(string streamName = null)
    {
      if(!wasRegistered)
      {
        wasRegistered = true;

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