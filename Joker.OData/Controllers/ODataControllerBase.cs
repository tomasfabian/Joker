using Dynamitey;
using Joker.Contracts.Data;
using Joker.OData.Extensions.Types;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Joker.OData.Extensions.Http;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Deltas;

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

    public async Task<IActionResult> Post([FromBody] TEntity entity)
    {
      var validationResult = await ValidatePostAsync(entity);

      if (validationResult != null)
        return validationResult;

      var actionResult = await OnPostAsync(entity);

      if (actionResult != null)
        return actionResult;

      return Created(entity);
    }

    protected virtual Task<IActionResult> ValidatePostAsync(TEntity entity)
    {
      return ValidateModelStateAsync(entity);
    }

    protected virtual async Task<IActionResult> OnPostAsync(TEntity entity)
    {
      repository.Add(entity);

      await repository.SaveChangesAsync();

      return null;
    }

    #endregion

    #region Patch

    [HttpPatch]
    public async Task<IActionResult> Patch(object key, Delta<TEntity> delta)
    {
      var keys = Request.GetKeysFromODataPath();

      var entityToUpdate = repository.GetAll().FirstOrDefault(CreateKeysPredicate(keys));

      if (entityToUpdate == null)
        return NotFound();

      delta.Patch(entityToUpdate);

      var validationResult = await ValidatePatchAsync(delta, entityToUpdate);

      if (validationResult != null)
        return validationResult;

      var actionResult = await OnPatchAsync(delta, entityToUpdate);

      if (actionResult != null)
        return actionResult;

      return Updated(entityToUpdate);
    }

    protected virtual async Task<IActionResult> OnPatchAsync(Delta<TEntity> delta, TEntity patchedEntity)
    {
      repository.Update(patchedEntity);

      await repository.SaveChangesAsync();

      return null;
    }

    protected virtual Task<IActionResult> ValidatePatchAsync(Delta<TEntity> delta, TEntity patchedEntity)
    {
      return ValidateModelStateAsync(patchedEntity);
    }

    #endregion

    #region Put

    [HttpPut]
    public async Task<IActionResult> Put(object key, [FromBody] TEntity entity)
    {
      var validationResult = await ValidatePutAsync(entity);

      if (validationResult != null)
        return validationResult;

      var actionResult = await OnPutAsync(entity);

      if (actionResult != null)
        return actionResult;

      return Updated(entity);
    }

    protected virtual async Task<IActionResult> OnPutAsync(TEntity entity)
    {
      repository.Update(entity);

      await repository.SaveChangesAsync();

      return null;
    }

    protected virtual Task<IActionResult> ValidatePutAsync(TEntity entity)
    {
      return ValidateModelStateAsync(entity);
    }

    #endregion

    #region Delete

    [HttpDelete]
    public async Task<IActionResult> Delete(object key, ODataOptions oDataOptions)
    {
      return await Delete();
    }

    public async Task<IActionResult> Delete()
    {
      var keys = Request.GetKeysFromODataPath();

      var validationResult = await ValidateDeleteAsync(keys);

      if (validationResult != null)
        return validationResult;

      var actionResult = await OnDeleteAsync(keys);

      if (actionResult != null)
        return actionResult;

      return StatusCode((int)HttpStatusCode.NoContent);
    }

    protected virtual async Task<IActionResult> OnDeleteAsync(params object[] keys)
    {
      repository.Remove(keys);

      await repository.SaveChangesAsync();

      return null;
    }

    protected virtual Task<IActionResult> ValidateDeleteAsync(object[] keys)
    {
      return Task.FromResult<IActionResult>(null);
    }

    #endregion

    #region TryGetDbSet

    protected virtual dynamic TryGetDbSet(Type entityType)
    {
      return null;
    }

    #endregion

    #region CreateRef

    [AcceptVerbs("POST", "PUT")]
    public async Task<IActionResult> CreateRef(object key, string navigationProperty, [FromBody] Uri link)
    {
      return await OnCreateRef(navigationProperty, link);
    }

    protected virtual async Task<IActionResult> OnCreateRef(string navigationProperty, Uri link)
    {
      var keys = Request.GetKeysFromODataPath();
      var keyPredicate = CreateKeysPredicate(keys);
      var entity = GetAll().Where(keyPredicate).FirstOrDefault(); //TODO: use Find in case of disabled change tracking?  

      if (entity == null)
        return NotFound($"{nameof(TEntity)}: {keys}");

      var odataPath = Request.GetODataPath(link);
      
      var relatedObjectKeys = ODataPathHelpers.GetKeysFromPath(odataPath);

      var type = typeof(TEntity).GetProperty(navigationProperty).PropertyType;

      var navigationPropertyType = type.GetCollectionGenericType() ?? type;

      dynamic relatedRepository = TryGetDbSet(navigationPropertyType);
      dynamic relatedEntity = relatedRepository.Find(relatedObjectKeys);

      if (typeof(ICollection).IsAssignableFrom(type))
      {
        dynamic dynamicNavigationProperty = Dynamic.InvokeGet(entity, navigationProperty);

        dynamicNavigationProperty.Add(relatedEntity);
      }
      else
        Dynamic.InvokeSet(entity, navigationProperty, relatedEntity);

      await repository.SaveChangesAsync();

      return StatusCode((int)HttpStatusCode.NoContent);
    }

    #endregion

    #region DeleteRef

    [HttpDelete]
    public async Task<IActionResult> DeleteRef(object key, string navigationProperty)
    {
      return await OnDeleteRef(navigationProperty);
    }

    protected virtual async Task<IActionResult> OnDeleteRef(string navigationProperty)
    {
      var keys = Request.GetKeysFromODataPath();
      var keyPredicate = CreateKeysPredicate(keys);
      var entity = repository.GetAllIncluding(navigationProperty).Where(keyPredicate).FirstOrDefault();

      if (entity == null)
        return NotFound($"{nameof(TEntity)}: {keys}");

      var type = typeof(TEntity).GetProperty(navigationProperty).PropertyType;

      var navigationPropertyType = type.GetCollectionGenericType();

      if (navigationPropertyType == null)
        navigationPropertyType = type;

      if (typeof(ICollection).IsAssignableFrom(type))
      {
        var relatedObjectKeys = Request.GetAllKeysFromODataPath();

        dynamic relatedRepository = TryGetDbSet(navigationPropertyType);
        dynamic relatedEntity = relatedRepository.Find(relatedObjectKeys);

        dynamic dynamicNavigationProperty = Dynamic.InvokeGet(entity, navigationProperty);

        dynamicNavigationProperty.Remove(relatedEntity);
      }
      else
        Dynamic.InvokeSet(entity, navigationProperty, null);

      await repository.SaveChangesAsync();

      return StatusCode((int)HttpStatusCode.NoContent);
    }

    #endregion

    #region ValidateModelStateAsync

    protected virtual Task<IActionResult> ValidateModelStateAsync(TEntity entity)
    {
      if (!ModelState.IsValid)
      {
        IActionResult badRequest = BadRequest(ModelState);

        return Task.FromResult(badRequest);
      }

      return Task.FromResult<IActionResult>(null);
    }

    #endregion
  }
}