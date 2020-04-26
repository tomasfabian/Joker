using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Joker.Contracts.Data;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Query.Validators;
using Microsoft.AspNetCore.Mvc;

namespace Joker.OData.Controllers
{
  public abstract class ODataControllerBase<TEntity> : ODataController
    where TEntity : class, Domain.IDomainEntity
  {
    #region Fields

    private readonly IRepository<TEntity> repository;

    #endregion

    #region Constructors

    protected ODataControllerBase(IRepository<TEntity> repository)
    {
      this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    
    #endregion
    
    #region Properties

    public FilterQueryValidator FilterQueryValidator { get; set; }
    
    public SelectExpandQueryValidator SelectExpandQueryValidator { get; set; }

    #endregion

    #region Get

    [EnableQuery(MaxExpansionDepth = 3)]
    public OkObjectResult Get(ODataQueryOptions<TEntity> queryOptions)
    {
      AuthenticateQuery(queryOptions);

      var entities = OnGetAll(queryOptions);

      return Ok(entities);
    }

    protected virtual IQueryable<TEntity> OnGetAll(ODataQueryOptions<TEntity> queryOptions)
    {
      return repository.GetAll();
    }

    protected IQueryable<TEntity> GetAll(ODataQueryOptions<TEntity> queryOptions)
    {
      return repository.GetAll();
    }

    public ObjectResult Get([FromODataUri] int key, ODataQueryOptions<TEntity> queryOptions)
    {
      AuthenticateQuery(queryOptions);

      var entity = OnGet(key, queryOptions);

      if (entity == null)
        return NotFound(key);

      return Ok(entity);
    }

    protected virtual TEntity OnGet(int key, ODataQueryOptions<TEntity> queryOptions)
    {
      return GetAll(queryOptions).FirstOrDefault(c => c.Id == key);
    }

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

    protected virtual Task<int> OnPost(TEntity entity)
    {
      repository.Add(entity);

      return repository.SaveChangesAsync();
    }

    #endregion

    #region Patch

    public async Task<IActionResult> Patch(Delta<TEntity> entity)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var id = entity.GetInstance().Id;
      var entityToUpdate = repository.GetAll().FirstOrDefault(c => c.Id == id);

      if (entityToUpdate == null)
        return NotFound();

      entity.Patch(entityToUpdate);

      var result = await OnPut(entityToUpdate);

      return Updated(entityToUpdate);
    }

    #endregion

    #region Put

    public async Task<IActionResult> Put([FromBody]TEntity entity)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      await OnPut(entity);

      return Updated(entity);
    }

    protected virtual Task<int> OnPut(TEntity entity)
    {
      repository.Update(entity);

      return repository.SaveChangesAsync();
    }

    #endregion

    #region Delete

    public async Task<IActionResult> Delete([FromODataUri] int key)
    {
      var result = await OnDelete(key);

      return StatusCode((int)HttpStatusCode.NoContent);
    }

    #endregion

    #region OnDelete
    
    protected virtual Task<int> OnDelete(int key)
    {
      repository.Remove(key);

      return repository.SaveChangesAsync();
    }

    #endregion

    private void AuthenticateQuery(ODataQueryOptions<TEntity> queryOptions)
    {
      ValidateQueryOptions(queryOptions);
    }    
    
    #region ValidateQueryOptions

    protected void ValidateQueryOptions(ODataQueryOptions<TEntity> queryOptions)
    {
      OnValidateQueryOptions(queryOptions);

      queryOptions.Validate(new ODataValidationSettings { MaxExpansionDepth = 3 });
    }

    #endregion

    #region OnValidateQueryOptions

    protected virtual void OnValidateQueryOptions(ODataQueryOptions<TEntity> queryOptions)
    {
      if (SelectExpandQueryValidator != null && queryOptions.SelectExpand != null)
      {
        queryOptions.SelectExpand.Validator = SelectExpandQueryValidator;
      }

      if (FilterQueryValidator != null && queryOptions.Filter != null)
      {
        queryOptions.Filter.Validator = FilterQueryValidator;
      }
    }

    #endregion
  }
}