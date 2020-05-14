using Joker.Contracts.Data;
using Joker.OData.Controllers;
using Sample.Domain.Models;

namespace SelfHostedODataService.EFCore.Controllers
{
  public class ProductsController : ODataControllerBase<Product>
  {
    public ProductsController(IRepository<Product> repository)
      : base(repository)
    {
    }
  }
}