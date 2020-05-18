using System;
using DataAccessLayer.Context;
using Domain.Models;
using Joker.Contracts.Data;
using Joker.OData.Controllers;

namespace SelfHostedODataService.Joker.EF.Controllers
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