using System;
using DataAccessLayer.EFCore;
using Domain.Models;
using Joker.Contracts.Data;
using Joker.OData.Controllers;

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