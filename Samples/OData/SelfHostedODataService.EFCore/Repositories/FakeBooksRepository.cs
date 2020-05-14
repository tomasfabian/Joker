using System.Linq;
using Joker.Contracts.Data;
using Sample.Domain.Models;

namespace SelfHostedODataService.EFCore.Repositories
{
  public class FakeBooksRepository : IReadOnlyRepository<Book>
  {
    public IQueryable<Book> GetAll()
    {
      return new[]
      {
        new Book {Title = "Title 1", Id = "dfkjdafaj"},
        new Book {Title = "Title 1", Id = "foajodjgo"}
      }.AsQueryable();
    }
  }
}