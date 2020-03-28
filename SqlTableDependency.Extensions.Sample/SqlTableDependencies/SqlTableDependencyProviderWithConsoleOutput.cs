using System;
using System.Configuration;
using System.Reactive.Concurrency;
using Joker.Domain;
using Sample.Domain.Models;
using SqlTableDependency.Extensions.Enums;
using SqlTableDependency.Extensions.Sample.Logging;
using TableDependency.SqlClient.Base;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;

namespace SqlTableDependency.Extensions.Sample.SqlTableDependencies
{
  internal abstract class SqlTableDependencyProviderWithConsoleOutput<TEntity> : SqlTableDependencyProvider<TEntity>
    where TEntity : DomainEntity, new()
  {
    #region Fields

    private readonly ILogger logger;

    #endregion

    #region Constructors

    internal SqlTableDependencyProviderWithConsoleOutput(ConnectionStringSettings connectionStringSettings, IScheduler scheduler, ILogger logger, LifetimeScope lifetimeScope)
      : base(connectionStringSettings, scheduler, lifetimeScope)
    {
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    internal SqlTableDependencyProviderWithConsoleOutput(string connectionString, IScheduler scheduler, ILogger logger, LifetimeScope lifetimeScope)
      : base(connectionString, scheduler, lifetimeScope)
    {
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #endregion

    #region Properties

    #region IsDatabaseAvailable

    protected override bool IsDatabaseAvailable
    {
      get
      {
        Console.WriteLine("Checking database connection...");

        bool isDatabaseAvailable =  base.IsDatabaseAvailable;

        Console.WriteLine($"IsDatabaseAvailable: {isDatabaseAvailable}");

        Console.WriteLine(Environment.NewLine);

        return isDatabaseAvailable;
      }
    }

    #endregion

    #endregion

    #region Methods

    #region OnInitializeMapper

    protected override ModelToTableMapper<TEntity> OnInitializeMapper(ModelToTableMapper<TEntity> modelToTableMapper)
    {
      modelToTableMapper.AddMapping(c => c.Id, nameof(DomainEntity.Id));

      return modelToTableMapper;
    }

    #endregion

    #region OnCreateSettings

    protected override SqlTableDependencySettings<TEntity> OnCreateSettings()
    {
      var settings =  base.OnCreateSettings();

      settings.SchemaName = "dbo";
      settings.UpdateOf = null;
      settings.Filter = null;
      settings.NotifyOn = DmlTriggerType.All;
      settings.ExecuteUserPermissionCheck = true;
      settings.IncludeOldValues = false;

      return settings;
    }

    #endregion

    #region OnInserted

    protected override void OnInserted(TEntity entity)
    {
      base.OnInserted(entity);

      logger.Trace("Entity was added");

      LogChangeInfo(entity);
    }

    #endregion

    #region OnUpdated

    protected override void OnUpdated(TEntity entity)
    {
      base.OnUpdated(entity);

      logger.Trace("Entity was modified");

      LogChangeInfo(entity);
    }

    #endregion

    #region OnDeleted

    protected override void OnDeleted(TEntity entity)
    {
      base.OnDeleted(entity);

      logger.Trace("Entity was deleted");

      LogChangeInfo(entity);
    }

    #endregion

    #region SqlTableDependencyOnStatusChanged

    protected override void SqlTableDependencyOnStatusChanged(object sender, StatusChangedEventArgs e)
    {
      base.SqlTableDependencyOnStatusChanged(sender, e);
      
      Console.WriteLine(Environment.NewLine);

      Console.WriteLine($"Status changed {e.Status}");
    }

    #endregion

    #region OnConnected

    protected override void OnConnected()
    {
      base.OnConnected();

      Console.WriteLine(Environment.NewLine);

      Console.WriteLine("Subscription started");
    }

    #endregion

    #region OnBeforeServiceBrokerSubscription

    protected override void OnBeforeServiceBrokerSubscription()
    {
      base.OnBeforeServiceBrokerSubscription();

      Console.WriteLine(Environment.NewLine);

      Console.WriteLine("Trying to reconnect...");
    }

    #endregion

    #region OnError

    protected override void OnError(Exception exception)
    {
      base.OnError(exception);

      Console.WriteLine(Environment.NewLine);
      Console.ForegroundColor = ConsoleColor.DarkRed;

      Console.WriteLine("Error: ");
      Console.WriteLine(exception.Message);

      Console.ResetColor();
    }

    #endregion

    #region GetDescription

    protected abstract string GetDescription(TEntity entity);

    #endregion

    #region LogChangeInfo

    private void LogChangeInfo(TEntity entity)
    {
      Console.WriteLine(Environment.NewLine);

      Console.WriteLine($"Id: {entity.Id}");
      Console.WriteLine($"Description: {GetDescription(entity)}");

      Console.WriteLine("#####");
      Console.WriteLine(Environment.NewLine);
    }

    #endregion

    #endregion
  }
}