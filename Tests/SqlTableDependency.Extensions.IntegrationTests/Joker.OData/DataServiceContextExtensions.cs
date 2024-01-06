using System.Data.Entity.Infrastructure.Pluralization;
using Microsoft.OData.Client;

namespace SqlTableDependency.Extensions.IntegrationTests.Joker.OData
{
  public static class DataServiceContextExtensions
  {
    public static void AddObject<TEntity>(this DataServiceContext dataServiceContext, TEntity entity)
    {
      var entitySetName = new EnglishPluralizationService().Pluralize(typeof(TEntity).Name);

      dataServiceContext.AddObject(entitySetName, entity);
    }
  }
}