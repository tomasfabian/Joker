using System;
using Joker.Contracts.Data;
using Joker.OData.Controllers;
using Sample.Domain.Models;

namespace SelfHostedODataService.EFCore.Controllers
{
  public class BooksController : ODataControllerBase<Book>
  {
    private readonly ISampleDbContext dbContext;

    public BooksController(IRepository<Book> repository, ISampleDbContext dbContext)
      : base(repository)
    {
      this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    protected override dynamic TryGetDbSet(Type entityType)
    {
      if (entityType == typeof(Publisher))
        return dbContext.Publishers;
      
      return base.TryGetDbSet(entityType);
    }
  }
}