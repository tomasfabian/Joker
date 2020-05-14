using System;
using System.Threading;
using System.Threading.Tasks;
using Joker.Redis.SqlTableDependency;
using Microsoft.Extensions.Hosting;
using Sample.Domain.Models;
using SqlTableDependency.Extensions;

namespace SelfHostedODataService.EFCore.HostedServices
{
  internal class SqlTableDependencyProviderHostedService : IHostedService
  {
    private readonly IHostApplicationLifetime appLifetime;
    private readonly ISqlTableDependencyProvider<Product> sqlTableDependencyProvider;
    private readonly ISqlTableDependencyRedisProvider<Product> sqlTableDependencyRedisProvider;

    public SqlTableDependencyProviderHostedService(
      IHostApplicationLifetime appLifetime,
      ISqlTableDependencyProvider<Product> sqlTableDependencyProvider,
      ISqlTableDependencyRedisProvider<Product> sqlTableDependencyRedisProvider)
    {
      this.appLifetime = appLifetime ?? throw new ArgumentNullException(nameof(appLifetime));
      this.sqlTableDependencyProvider = sqlTableDependencyProvider ?? throw new ArgumentNullException(nameof(sqlTableDependencyProvider));
      this.sqlTableDependencyRedisProvider = sqlTableDependencyRedisProvider ?? throw new ArgumentNullException(nameof(sqlTableDependencyRedisProvider));
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

    private void InitializeSqlTableDependencyRedisProvider()
    {
      sqlTableDependencyProvider.SubscribeToEntityChanges();
      
      sqlTableDependencyRedisProvider.StartPublishing();
    }

    private void OnStopping()
    {
      sqlTableDependencyRedisProvider?.Dispose();
    }

    private void OnStopped()
    {

    }
  }
}