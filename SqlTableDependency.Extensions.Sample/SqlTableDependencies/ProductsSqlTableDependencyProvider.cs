using System;
using System.Configuration;
using System.Reactive.Concurrency;
using Sample.Domain.Models;
using SqlTableDependency.Extensions.Enums;
using SqlTableDependency.Extensions.Sample.Logging;

namespace SqlTableDependency.Extensions.Sample.SqlTableDependencies
{
  internal class ProductsSqlTableDependencyProvider : SqlTableDependencyProviderWithConsoleOutput<Product>
  { 
    #region Constructors

    internal ProductsSqlTableDependencyProvider(ConnectionStringSettings connectionStringSettings, IScheduler scheduler, ILogger logger)
      : base(connectionStringSettings, scheduler, logger, LifetimeScope.ApplicationScope)
    {
    }

    internal ProductsSqlTableDependencyProvider(string connectionString, IScheduler scheduler, ILogger logger)
      : base(connectionString, scheduler, logger, LifetimeScope.ConnectionScope)
    {
    }

    #endregion

    #region GetDescription

    protected override string GetDescription(Product entity)
    {
      return entity.Name;
    }

    #endregion
  }
}