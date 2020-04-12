using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;

namespace SelfHostedODataService.Controllers
{
  public abstract class ODataControllerBase<TEntity> : ODataController
    where TEntity : class, Joker.Domain.IDomainEntity
  {   
    #region Get

    [EnableQuery(MaxExpansionDepth = 3)]
    public OkObjectResult Get(ODataQueryOptions<TEntity> queryOptions)
    {
      var entities = GetAll();

      return Ok(entities);
    }

    protected abstract IQueryable<TEntity> GetAll();

    #endregion

    #region Post

    public async Task<IActionResult> Post([FromBody]TEntity entity)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      await OnPost(entity);

      return Created(entity);
    }

    protected abstract Task<int> OnPost(TEntity entity);

    #endregion

    #region Put

    public IActionResult Put([FromBody]TEntity entity)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      
      return Updated(entity);
    }

    #endregion
  }
}