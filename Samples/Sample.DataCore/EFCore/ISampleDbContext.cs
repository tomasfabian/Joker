using Microsoft.EntityFrameworkCore;
using Sample.Domain.Models;

namespace Sample.DataCore.EFCore
{
  public interface ISampleDbContext
  {
    DbSet<Product> Products { get; set; }
    DbSet<Book> Books { get; set; }
    DbSet<Author> Authors { get; set; }
    DbSet<Publisher> Publishers { get; set; }
  }
}