﻿using Dynamitey;
using Joker.Contracts.Data;
using Joker.OData.Extensions.OData;
using Joker.OData.Extensions.Types;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

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

    public async Task<IActionResult> Delete()
    {
      var keys = GetKeysFromPath();

      var result = await OnDelete(keys);

      return StatusCode((int)HttpStatusCode.NoContent);
    }

    #endregion

    #region OnDelete

    protected virtual Task<int> OnDelete(params object[] keys)
    {
      repository.Remove(keys);

      return repository.SaveChangesAsync();
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
    public async Task<IActionResult> CreateRef(string navigationProperty, [FromBody] Uri link)
    {
      return await OnCreateRef(navigationProperty, link);
    }

    protected virtual async Task<IActionResult> OnCreateRef(string navigationProperty, Uri link)
    {
      var keys = GetKeysFromPath();
      var keyPredicate = CreateKeysPredicate(keys);
      var entity = GetAll().Where(keyPredicate).FirstOrDefault();

      if (entity == null)
        return NotFound($"{nameof(TEntity)}: {keys}");


      var odataPath = Request.CreateODataPath(link);

      var relatedObjectKeys = GetKeysFromPath(odataPath);

      var type = typeof(TEntity).GetProperty(navigationProperty).PropertyType;

      var navigationPropertyType = type.GetCollectionGenericType();

      if (navigationPropertyType == null)
        navigationPropertyType = type;

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

      return StatusCode((int) HttpStatusCode.NoContent);
    }

    #endregion

    #region DeleteRef

    public async Task<IActionResult> DeleteRef(string navigationProperty)
    {
      return await OnDeleteRef(navigationProperty);
    }

    protected virtual async Task<IActionResult> OnDeleteRef(string navigationProperty)
    {      
      var keys = GetKeysFromPath();
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
        var relatedObjectKeys = GetAllKeysFromPath().Last().Select(c => c.Value).ToArray();

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
  }
}