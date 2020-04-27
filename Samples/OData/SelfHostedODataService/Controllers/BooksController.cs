using Joker.Contracts.Data;
using Joker.OData.Controllers;
using Sample.Domain.Models;

namespace SelfHostedODataService.Controllers
{
  public class BooksController : ReadOnlyODataController<Book>
  {
    public BooksController(IReadOnlyRepository<Book> repository)
      : base(repository)
    {
    }
  }
}