using System.Threading;
using System.Threading.Tasks;
using Joker.Factories.Schedulers;
using Joker.Redis.ConnectionMultiplexers;
using Microsoft.Extensions.Hosting;
using Sample.Data.SqlTableDependencyProvider;
using SelfHostedODataService.Configuration;
using SelfHostedODataService.Redis;
using SqlTableDependency.Extensions.Enums;

namespace SelfHostedODataService.HostedServices
{
  internal class SqlTableDependencyProviderHostedService : IHostedService
  {
    private readonly IHostApplicationLifetime appLifetime;

    public SqlTableDependencyProviderHostedService(
      IHostApplicationLifetime appLifetime)
    {
      this.appLifetime = appLifetime;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      appLifetime.ApplicationStarted.Register(OnStarted);
      appLifetime.ApplicationStopping.Register(OnStopping);
      appLifetime.ApplicationStopped.Register(OnStopped);

      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      return Task.CompletedTask;
    }

    private void OnStarted()
    {
      InitializeSqlTableDependencyRedisProvider();
    }

    #region InitializeSqlTableDependencyRedisProvider

    private ProductSqlTableDependencyRedisProvider redisPublisher;

    private void InitializeSqlTableDependencyRedisProvider()
    {
      string connectionString = ConfigurationProvider.GetDatabaseConnectionString();

      var schedulersFactory = new SchedulersFactory();
      var productsChangesProvider =
        new ProductsSqlTableDependencyProvider(connectionString, schedulersFactory.TaskPool, LifetimeScope.UniqueScope);
      productsChangesProvider.SubscribeToEntityChanges();


      string redisUrl = ConfigurationProvider.RedisUrl;
      
      redisPublisher = new ProductSqlTableDependencyRedisProvider(productsChangesProvider,
        new RedisPublisher(redisUrl), schedulersFactory.TaskPool);
      
      redisPublisher.StartPublishing();
    }

    #endregion

    private void OnStopping()
    {
      redisPublisher?.Dispose();
    }

    private void OnStopped()
    {

    }
  }
}