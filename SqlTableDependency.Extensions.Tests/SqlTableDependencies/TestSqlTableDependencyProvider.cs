using System;
using System.Configuration;
using System.Reactive.Concurrency;
using SqlTableDependency.Extensions.Tests.Models;
using TableDependency.SqlClient.Base.Abstracts;

namespace SqlTableDependency.Extensions.Tests.SqlTableDependencies
{
  public class TestSqlTableDependencyProvider : SqlTableDependencyProvider<TestModel>
  {
    private readonly ITableDependency<TestModel> tableDependency;

    public TestSqlTableDependencyProvider(ConnectionStringSettings connectionStringSettings, IScheduler scheduler, ITableDependency<TestModel> tableDependency)
      : base(connectionStringSettings, scheduler)
    {
      this.tableDependency = tableDependency ?? throw new ArgumentNullException(nameof(tableDependency));
    }

    public TestSqlTableDependencyProvider(string connectionString, IScheduler scheduler, ITableDependency<TestModel> tableDependency) 
      : base(connectionString, scheduler)
    {
      this.tableDependency = tableDependency ?? throw new ArgumentNullException(nameof(tableDependency));
    }

    protected override ITableDependency<TestModel> CreateSqlTableDependency(IModelToTableMapper<TestModel> modelToTableMapper)
    {
      return tableDependency;
    }

    protected override bool IsDatabaseAvailable => true;
  }
}