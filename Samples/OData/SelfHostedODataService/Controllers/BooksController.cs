using System;
using System.Data.Entity;
using Joker.Contracts.Data;
using Joker.OData.Controllers;
using Sample.Data.Context;
using Sample.Domain.Models;

namespace SelfHostedODataService.Controllers
{
  public class BooksController : ODataControllerBase<Book>
  {
    private readonly ISampleDbContext dbContext;

    public BooksController(IRepository<Book> repository, ISampleDbContext dbContext)
      : base(repository)
    {
      this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }
  }
}