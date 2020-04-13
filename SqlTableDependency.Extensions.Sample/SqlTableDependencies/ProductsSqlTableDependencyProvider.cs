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
    private readonly ILogger logger;

    #region Constructors

    internal ProductsSqlTableDependencyProvider(ConnectionStringSettings connectionStringSettings, IScheduler scheduler, ILogger logger)
      : base(connectionStringSettings, scheduler, logger, LifetimeScope.ApplicationScope)
    {
    }

    internal ProductsSqlTableDependencyProvider(string connectionString, IScheduler scheduler, ILogger logger)
      : base(connectionString, scheduler, logger, LifetimeScope.UniqueScope)
    {
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #endregion
    
    #region TableName

    protected override string TableName => base.TableName+"s";

    #endregion

    #region GetDescription

    protected override string GetDescription(Product entity)
    {
      return entity.Name;
    }

    #endregion

    protected override SqlTableDependencySettings<Product> OnCreateSettings()
    {
      var settings = base.OnCreateSettings();

      settings.IncludeOldValues = true;

      return settings;
    }

    #region OnUpdated

    protected override void OnUpdated(Product entity, Product oldValues)
    {
      base.OnUpdated(entity, oldValues);

      if (oldValues != null)
      {
        logger.Trace("Entity old values");

        LogChangeInfo(oldValues);
      }
    }

    #endregion
  }
}