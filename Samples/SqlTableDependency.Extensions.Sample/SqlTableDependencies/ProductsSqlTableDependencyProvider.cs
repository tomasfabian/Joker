using System;
using System.Reactive.Concurrency;
using Joker.Domain;
using Sample.Domain.Models;
using SqlTableDependency.Extensions.Enums;
using SqlTableDependency.Extensions.Sample.Logging;

namespace SqlTableDependency.Extensions.Sample.SqlTableDependencies
{
  internal class ProductsSqlTableDependencyProvider : ProductsSqlTableDependencyProvider<Product>
  {
    internal ProductsSqlTableDependencyProvider(string connectionString, IScheduler scheduler, ILogger logger) 
      : base(connectionString, scheduler, logger)
    {
    }
    
    #region TableName

    protected override string TableName => base.TableName+"s";

    #endregion

    #region GetDescription

    protected override string GetDescription(Product entity)
    {
      return entity.Name;
    }

    #endregion
  }

  internal abstract class ProductsSqlTableDependencyProvider<TProduct> : SqlTableDependencyProviderWithConsoleOutput<TProduct> 
    where TProduct : DomainEntity, new()
  {
    #region Fields

    private readonly ILogger logger;

    #endregion

    #region Constructors

    internal ProductsSqlTableDependencyProvider(string connectionString, IScheduler scheduler, ILogger logger, LifetimeScope lifetimeScope = LifetimeScope.UniqueScope)
      : base(connectionString, scheduler, logger, lifetimeScope)
    {
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #endregion

    protected override SqlTableDependencySettings<TProduct> OnCreateSettings()
    {
      var settings = base.OnCreateSettings();

      settings.IncludeOldValues = true;

      return settings;
    }

    #region OnUpdated

    protected override void OnUpdated(TProduct entity, TProduct oldValues)
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