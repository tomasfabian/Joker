using System;
using System.Configuration;
using System.Reactive.Concurrency;
using Moq;
using SqlTableDependency.Extensions.Providers.Sql;
using SqlTableDependency.Extensions.Tests.Models;
using TableDependency.SqlClient.Base.Abstracts;

namespace SqlTableDependency.Extensions.Tests.SqlTableDependencies
{
  public class TestSqlTableDependencyProvider : SqlTableDependencyProvider<TestModel>
  {
    private readonly ITableDependency<TestModel> tableDependency;

    public TestSqlTableDependencyProvider(ConnectionStringSettings connectionStringSettings, IScheduler scheduler, ITableDependency<TestModel> tableDependency, bool preserveDatabaseObjects = true)
      : this(connectionStringSettings.ConnectionString, scheduler, tableDependency, preserveDatabaseObjects)
    {
      this.tableDependency = tableDependency ?? throw new ArgumentNullException(nameof(tableDependency));
    }

    public TestSqlTableDependencyProvider(string connectionString, IScheduler scheduler, ITableDependency<TestModel> tableDependency, bool preserveDatabaseObjects = true) 
      : base(connectionString, scheduler, preserveDatabaseObjects)
    {
      this.tableDependency = tableDependency ?? throw new ArgumentNullException(nameof(tableDependency));

      sqlConnectionProviderMock.Setup(c => c.TestConnection(connectionString, It.IsAny<TimeSpan>()))
        .Returns<string, TimeSpan>((s,t) => IsDatabaseAvailableTestOverride);
    }
    
    #region SqlConnectionProvider

    private readonly Mock<ISqlConnectionProvider> sqlConnectionProviderMock = new Mock<ISqlConnectionProvider>();

    protected override ISqlConnectionProvider SqlConnectionProvider => sqlConnectionProviderMock.Object;

    #endregion

    public bool IsDatabaseAvailableTestOverride { get; set; } = true;

    protected override ITableDependency<TestModel> CreateSqlTableDependency(IModelToTableMapper<TestModel> modelToTableMapper)
    {
      return tableDependency;
    }
  }
}