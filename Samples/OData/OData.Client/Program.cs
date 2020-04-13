using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Client;
using Sample.Domain.Models;

namespace OData.Client
{
  class Program
  {
    static async Task Main(string[] args)
    {
      var dataServiceContext = new ODataServiceContextFactory().Create(@"https://localhost:44364/");
      var products = await ((DataServiceQuery<Product>) dataServiceContext.Products.Take(3)).ExecuteAsync();

      var product = new Product {Name = "OData"};

      dataServiceContext.AddObject("Products", product);
      
      await dataServiceContext.SaveChangesAsync();

      product.Name = "OData renamed";
      dataServiceContext.UpdateObject(product);

      await dataServiceContext.SaveChangesAsync();

      dataServiceContext.DeleteObject(product);
      
      await dataServiceContext.SaveChangesAsync();

      Console.ReadKey();
    }
  }
}