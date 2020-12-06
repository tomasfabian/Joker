using System;
using Joker.Contracts.Data;
using Joker.OData.Controllers;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Query.Validator;
using Sample.Data.Context;
using Sample.Domain.Models;

namespace SelfHostedODataService.Controllers
{
  public class PublishersController : ODataControllerBase<Publisher>
  {
    private readonly ISampleDbContext dbContext;

    public PublishersController(IRepository<Publisher> repository, ISampleDbContext dbContext)
      : base(repository)
    {
      this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    protected override dynamic TryGetDbSet(Type entityType)
    {
      if (entityType == typeof(Book))
        return dbContext.Books;
      
      return base.TryGetDbSet(entityType);
    }

    protected override ODataValidationSettings OnCreateODataValidationSettings()
    {
      var oDataValidationSettings = base.OnCreateODataValidationSettings();

      oDataValidationSettings.MaxExpansionDepth = 3; //default is 2

      oDataValidationSettings.AllowedQueryOptions = //disabled AllowedQueryOptions.Format
        AllowedQueryOptions.Apply | AllowedQueryOptions.SkipToken | AllowedQueryOptions.Count
        | AllowedQueryOptions.Skip | AllowedQueryOptions.Top | AllowedQueryOptions.OrderBy
        | AllowedQueryOptions.Select | AllowedQueryOptions.Expand | AllowedQueryOptions.Filter;


      return oDataValidationSettings;
    }
  }
}