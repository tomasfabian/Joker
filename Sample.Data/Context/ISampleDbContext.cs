using System.Data.Entity;
using Sample.Domain.Models;

namespace Sample.Data.Context
{
  public interface ISampleDbContext
  {
    IDbSet<Product> Products { get; set; }
  }
}