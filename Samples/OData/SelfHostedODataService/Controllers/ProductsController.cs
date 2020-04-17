using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Sample.Data.Context;
using Sample.Domain.Models;

namespace SelfHostedODataService.Controllers
{
  public class ProductsController : ODataControllerBase<Product>
  {
    private readonly ISampleDbContext dbContext;

    public ProductsController(ISampleDbContext dbContext)
    {
      this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    protected override IQueryable<Product> GetAll()
    {
      return dbContext.Products;
    }

    protected override Task<int> OnPost(Product entity)
    {
      dbContext.Products.Add(entity);

      return dbContext.SaveChangesAsync();
    }

    protected override Task<int> OnPut(Product entity)
    {
      dbContext.Products.AddOrUpdate(entity);

      return dbContext.SaveChangesAsync();
    }

    protected override Task<int> OnDelete(int key)
    {
      var entity = dbContext.Products.Find(key);

      dbContext.Products.Remove(entity);

      return dbContext.SaveChangesAsync();
    }
  }
}