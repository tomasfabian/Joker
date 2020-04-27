using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Joker.Contracts.Data;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;

namespace Joker.OData.Controllers
{
  public abstract class ODataControllerBase<TEntity> : ReadOnlyODataController<TEntity>
      where TEntity : class
  {
    #region Fields

    private readonly IRepository<TEntity> repository;

    #endregion

    #region Constructors

    protected ODataControllerBase(IRepository<TEntity> repository)
      : base(repository)
    {
      this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    #endregion

    #region Post

    public async Task<IActionResult> Post([FromBody]TEntity entity)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

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
        return BadRequest(ModelState);

      var keys = GetKeysFromPath();

      var entityToUpdate = repository.GetAll().FirstOrDefault(CreateKeysPredicate(keys));

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
        return BadRequest(ModelState);

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
  }
}