using System;
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
  }
}