using Joker.Contracts.Data;
using Microsoft.EntityFrameworkCore;
using Sample.Domain.Models;

namespace Sample.DataCore.EFCore
{
  public interface ISampleDbContext : IContext, IDbTransactionFactory
  {
    DbSet<Product> Products { get; set; }

    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
  }
}