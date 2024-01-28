using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.OData.Client;

namespace SqlTableDependency.Extensions.IntegrationTests.Joker.OData
{
  public static class DataServiceQueryExtensions
  {
    public static async Task<int> EntitiesCount<TEntity>(this DataServiceQuery<TEntity> query)
    {
      var entities = await query.ExecuteAsync();

      int entitiesCount = entities.Count();

      return entitiesCount;
    }

    public static async Task<TEntity> FirstOrDefaultAsync<TEntity>(this DataServiceQuery<TEntity> query, Expression<Func<TEntity, bool>> predicate)
    {
      var entities = await ((DataServiceQuery<TEntity>) query.Where(predicate).Take(1)).ExecuteAsync();

      return entities.FirstOrDefault();
    }
  }
}