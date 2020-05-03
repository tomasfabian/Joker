using System;
using Joker.Contracts.Data;
using Joker.OData.Controllers;
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
  }
}