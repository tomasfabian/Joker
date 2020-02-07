using System;
using System.Configuration;
using System.Reactive.Concurrency;
using Sample.Domain.Models;
using SqlTableDependency.Extensions.Enums;
using SqlTableDependency.Extensions.Sample.Logging;
using TableDependency.SqlClient.Base;

namespace SqlTableDependency.Extensions.Sample.SqlTableDependencies
{
  internal class DevicesSqlTableDependencyProvider : SqlTableDependencyProviderWithConsoleOutput<Device>
  {
    #region Constructors

    internal DevicesSqlTableDependencyProvider(ConnectionStringSettings connectionStringSettings, IScheduler scheduler,
      ILogger logger)
      : base(connectionStringSettings, scheduler, logger, LifetimeScope.ConnectionScope)
    {
    }

    internal DevicesSqlTableDependencyProvider(string connectionString, IScheduler scheduler, ILogger logger)
      : base(connectionString, scheduler, logger, LifetimeScope.ConnectionScope)
    {
    }

    #endregion

    #region Methods

    #region OnInitializeMapper

    protected override ModelToTableMapper<Device> OnInitializeMapper(ModelToTableMapper<Device> modelToTableMapper)
    {
      modelToTableMapper.AddMapping(c => c.Id, "IdDevice");

      return modelToTableMapper;
    }

    #endregion

    #region GetDescription

    protected override string GetDescription(Device entity)
    {
      return $"{entity.Name} {entity.LastOnline}";
    }

    #endregion

    #endregion
  }
}