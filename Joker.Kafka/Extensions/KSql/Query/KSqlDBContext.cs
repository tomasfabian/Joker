using System;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi;
using Microsoft.Extensions.DependencyInjection;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  public sealed class KSqlDBContext
  {
    private readonly KSqlDBContextOptions contextOptions;
    private IServiceCollection serviceCollection;

    public KSqlDBContext(KSqlDBContextOptions contextOptions)
    {
      this.contextOptions = contextOptions ?? throw new ArgumentNullException(nameof(contextOptions));

      RegisterDependencies(contextOptions);

      BuildServicesProvider();
    }

    private void BuildServicesProvider()
    {
      var serviceProvider = serviceCollection.BuildServiceProvider(new ServiceProviderOptions {ValidateScopes = true});

      var provider = serviceProvider.GetService<IKSqlQbservableProvider>();
      var dbProvider = serviceProvider.GetService<IKSqldbProvider>();
    }

    private void RegisterDependencies(KSqlDBContextOptions contextOptions)
    {
      serviceCollection = new ServiceCollection();

      serviceCollection.AddTransient<IKSqlQbservableProvider, QbservableProvider>();

      var uri = new Uri(contextOptions.Url);

      serviceCollection.AddTransient<IKSqlQueryGenerator, KSqlQueryGenerator>();

      serviceCollection.AddTransient<IHttpClientFactory, HttpClientFactory>(s =>
        new HttpClientFactory(uri));

      serviceCollection.AddTransient<IKSqldbProvider, KSqlDbQueryStreamProvider>();
      serviceCollection.AddTransient<IKStreamSetDependencies, KStreamSetDependencies>();
    }

    private static ServiceProvider BuildServiceProvider(IServiceCollection services, bool validateScopes)
    {
      return services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = validateScopes });
    }

    public IQbservable<TEntity> CreateStreamSet<TEntity>()
    {
      var serviceProvider = serviceCollection.BuildServiceProvider(new ServiceProviderOptions {ValidateScopes = true});

      var dependencies = serviceProvider.GetService<IKStreamSetDependencies>();

      return new KQueryStreamSet<TEntity>(dependencies);
    }
  }
}