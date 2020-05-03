using System.Data.Entity;
using Joker.Contracts.Data;
using Sample.Domain.Models;

namespace Sample.Data.Context
{
  public interface ISampleDbContext : IContext, IDbTransactionFactory
  {
    IDbSet<Product> Products { get; set; }

    IDbSet<Book> Books { get; set; }
    IDbSet<Author> Authors { get; set; }
    IDbSet<Publisher> Publishers { get; set; }
  }
}