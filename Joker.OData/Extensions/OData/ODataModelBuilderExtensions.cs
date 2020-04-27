using System.Data.Entity.Infrastructure.Pluralization;
using Microsoft.AspNet.OData.Builder;

namespace Joker.OData.Extensions.OData
{
  public static class ODataModelBuilderExtensions
  {
    private static readonly EnglishPluralizationService EnglishPluralizationService = new EnglishPluralizationService();

    public static EntitySetConfiguration<TEntity> AddPluralizedEntitySet<TEntity>(this ODataModelBuilder oDataModelBuilder)
      where TEntity : class
    {
      string entitySetName = EnglishPluralizationService.Pluralize(typeof(TEntity).Name);

      return oDataModelBuilder.EntitySet<TEntity>(entitySetName);
    }
  }
}