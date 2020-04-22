using System.Reactive.Concurrency;
using SqlTableDependency.Extensions.Enums;
using SqlTableDependency.Extensions.Tests.Models;
using TableDependency.SqlClient.Base.Abstracts;

namespace SqlTableDependency.Extensions.Tests.SqlTableDependencies
{
  internal class TestModelWithTableAttributeSqlTableDependencyProvider : TestSqlTableDependencyProvider<TestModelWithTableAttribute>
  {
    public TestModelWithTableAttributeSqlTableDependencyProvider(string connectionString, IScheduler scheduler, ITableDependency<TestModelWithTableAttribute> tableDependency, LifetimeScope lifetimeScope) 
      : base(connectionString, scheduler, tableDependency, lifetimeScope)
    {
    }
  }
}