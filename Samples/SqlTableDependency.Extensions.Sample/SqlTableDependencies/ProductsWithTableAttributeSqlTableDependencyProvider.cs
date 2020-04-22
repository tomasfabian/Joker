using System.Reactive.Concurrency;
using Sample.Domain.Models;
using SqlTableDependency.Extensions.Enums;
using SqlTableDependency.Extensions.Sample.Logging;
using TableDependency.SqlClient.Base;

namespace SqlTableDependency.Extensions.Sample.SqlTableDependencies
{
  internal class ProductsWithTableAttributeSqlTableDependencyProvider : ProductsSqlTableDependencyProvider<ProductWithTableAttribute>
  {
    internal ProductsWithTableAttributeSqlTableDependencyProvider(string connectionString, IScheduler scheduler, ILogger logger) 
      : base(connectionString, scheduler, logger, LifetimeScope.ApplicationScope)
    {
    }
 
    #region GetDescription

    protected override string GetDescription(ProductWithTableAttribute entity)
    {
      return entity.ReNameD;
    }

    #endregion

    protected override ModelToTableMapper<ProductWithTableAttribute> OnInitializeMapper(ModelToTableMapper<ProductWithTableAttribute> modelToTableMapper)
    {
      return null;
    }
  }
}