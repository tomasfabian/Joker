using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Joker.Contracts.Data;
using Joker.Domain;
using Joker.OData.Controllers;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SelfHostedODataService.EFCore.Controllers
{
  public interface IUser
  {
    string UserId { get; }
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ECommerceUsers", Roles = "User")]
  public class ODataControllerWithAuth<TEntity> : ODataControllerBase<TEntity>
    where TEntity : class, IUser, IDomainEntity
  {
    private readonly IRepository<TEntity> repository;

    public ODataControllerWithAuth(IRepository<TEntity> repository) 
      : base(repository)
    {
      this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    
    protected string IdentityUserId => HttpContext.User.Claims
                                                        .Where(c => c.Type == ClaimTypes.NameIdentifier)
                                                        .Select(c => c.Value)
                                                        .SingleOrDefault();

    protected override Task<IActionResult> ValidatePostAsync(TEntity entity)
    {
      var result = ValidateUserId(entity);

      if (result != null)
        return result;

      return base.ValidatePostAsync(entity);
    }

    protected Task<IActionResult> ValidateUserId(TEntity entity)
    {
      if (entity.UserId != IdentityUserId)
      {
        IActionResult unauthorizedResult = Unauthorized();

        return Task.FromResult(unauthorizedResult);
      }

      return null;
    }

    protected override Task<IActionResult> ValidatePutAsync(TEntity entity)
    {
      var result = ValidateUserId(entity);

      if (result != null)
        return result;

      return base.ValidatePutAsync(entity);
    }

    protected override Task<IActionResult> ValidatePatchAsync(Delta<TEntity> delta, TEntity entity)
    {
      var result = ValidateUserId(entity);

      if (result != null)
        return result;

      return base.ValidatePatchAsync(delta, entity);
    }

    protected override async Task<IActionResult> ValidateDeleteAsync(object[] keys)
    {
      int id =  (int)keys[0];

      var entity = await repository.GetAll().FirstOrDefaultAsync(c => c.Id == id);

      var result = ValidateUserId(entity);

      if (result != null)
        return await result;
      
      return await base.ValidateDeleteAsync(keys);
    }
  }
}
