using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using NLog;
using Sample.Domain.Models;
using SqlTableDependency.Extensions;
using SqlTableDependency.Extensions.Enums;
using TableDependency.SqlClient.Base;
using TableDependency.SqlClient.Base.Abstracts;

namespace Sample.Data.SqlTableDependencyProvider
{
  public class ProductsSqlTableDependencyProvider : SqlTableDependencyProvider<Product>
  {
    #region Constructors

    public ProductsSqlTableDependencyProvider(string connectionString, IScheduler scheduler, LifetimeScope lifetimeScope)
      : base(connectionString, scheduler, lifetimeScope)
    {
    }

    #endregion

    #region Properties

    #region TableName

    protected override string TableName => base.TableName+"s";

    #endregion

    #endregion

    #region Methods

    #region OnInitializeMapper

    protected override ModelToTableMapper<Product> OnInitializeMapper(ModelToTableMapper<Product> modelToTableMapper)
    {
      modelToTableMapper.AddMapping(c => c.Id, nameof(Product.Id));

      return modelToTableMapper;
    }

    #endregion

    #region CreateSqlTableDependency

    private NLogTraceListener traceListener;

    protected override ITableDependency<Product> CreateSqlTableDependency(IModelToTableMapper<Product> modelToTableMapper)
    {
      var sqlTableDependency = base.CreateSqlTableDependency(modelToTableMapper);

      traceListener?.Dispose();

      sqlTableDependency.TraceListener = traceListener = new NLogTraceListener();
      sqlTableDependency.TraceLevel = TraceLevel.Verbose;

      return sqlTableDependency;
    }

    #endregion

    #region OnCreateSettings

    protected override SqlTableDependencySettings<Product> OnCreateSettings()
    {
      var settings = base.OnCreateSettings();

      settings.SchemaName = "dbo";
      settings.IncludeOldValues = true;

      return settings;
    }

    #endregion

    #region OnDispose

    protected override void OnDispose()
    {
      base.OnDispose();
      
      traceListener?.Dispose();
    }

    #endregion

    #endregion
  }
}