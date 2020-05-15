using Microsoft.AspNet.OData.Builder;
using Pluralize.NET;

namespace Joker.OData.Extensions.OData
{
  public static class ODataModelBuilderExtensions
  {
    private static readonly IPluralize EnglishPluralizationService = new Pluralizer();

    public static EntitySetConfiguration<TEntity> AddPluralizedEntitySet<TEntity>(this ODataModelBuilder oDataModelBuilder)
      where TEntity : class
    {
      string entitySetName = EnglishPluralizationService.Pluralize(typeof(TEntity).Name);

      return oDataModelBuilder.EntitySet<TEntity>(entitySetName);
    }
  }
}