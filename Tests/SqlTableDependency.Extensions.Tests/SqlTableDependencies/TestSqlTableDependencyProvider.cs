using System;
using System.Reactive.Concurrency;
using Moq;
using SqlTableDependency.Extensions.Enums;
using SqlTableDependency.Extensions.Providers.Sql;
using SqlTableDependency.Extensions.Tests.Models;
using TableDependency.SqlClient.Base.Abstracts;

namespace SqlTableDependency.Extensions.Tests.SqlTableDependencies
{
  internal class TestSqlTableDependencyProvider : TestSqlTableDependencyProvider<TestModel>
  {
    public TestSqlTableDependencyProvider(string connectionString, IScheduler scheduler, ITableDependency<TestModel> tableDependency, LifetimeScope lifetimeScope) 
      : base(connectionString, scheduler, tableDependency, lifetimeScope)
    {
    }
  }

  internal abstract class TestSqlTableDependencyProvider<TTestModel> : SqlTableDependencyProvider<TTestModel> 
    where TTestModel : class, new()
  {
    private readonly ITableDependency<TTestModel> tableDependency;

    protected TestSqlTableDependencyProvider(string connectionString, IScheduler scheduler, ITableDependency<TTestModel> tableDependency, LifetimeScope lifetimeScope) 
      : base(connectionString, scheduler, lifetimeScope)
    {
      this.tableDependency = tableDependency ?? throw new ArgumentNullException(nameof(tableDependency));

      sqlConnectionProviderMock.Setup(c => c.TestConnection(connectionString, It.IsAny<TimeSpan>()))
        .Returns<string, TimeSpan>((s,t) => IsDatabaseAvailableTestOverride);
    }
    
    #region SqlConnectionProvider

    private readonly Mock<ISqlConnectionProvider> sqlConnectionProviderMock = new();

    protected override ISqlConnectionProvider SqlConnectionProvider => sqlConnectionProviderMock.Object;

    #endregion

    public bool IsDatabaseAvailableTestOverride { get; set; } = true;

    internal new string TableName => base.TableName;

    protected override ITableDependency<TTestModel> CreateSqlTableDependency(IModelToTableMapper<TTestModel> modelToTableMapper)
    {
      return tableDependency;
    }

    private SqlTableDependencySettings<TTestModel> sqlTableDependencySettings;

    internal void SetSettings(SqlTableDependencySettings<TTestModel> tableDependencySettings)
    {
      sqlTableDependencySettings = tableDependencySettings;
    }

    protected override SqlTableDependencySettings<TTestModel> OnCreateSettings()
    {
      return sqlTableDependencySettings;
    }
  }
}