using System.Data.Entity;
using System.Threading.Tasks;
using Sample.Domain.Models;

namespace Sample.Data.Context
{
  public interface ISampleDbContext
  {
    IDbSet<Product> Products { get; set; }

    Task<int> SaveChangesAsync();
  }
}