using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Client;
using Sample.Domain.Models;

namespace OData.Client
{
  class Program
  {
    private static string url = @"http://localhost:3000/";

    static async Task Main(string[] args)
    {
      Console.WriteLine("Press any key to continue");
      Console.ReadKey();
      
      await Products_CRUD();

      await AddLink_Then_DeleteLink_CompoundKey();
      await SetLink_ThenRemoveLink_ThenUpdate_ThenDelete_CompoundKey();

      return;

      await Batch_UniqueKeyViolation();

      Console.ReadKey();
    }

    private static async Task Products_CRUD()
    {
      var dataServiceContext = new ODataServiceContextFactory().Create(url);
      var products = await ((DataServiceQuery<Product>) dataServiceContext.Products.Take(3)).ExecuteAsync();

      var product = new Product {Name = "OData"};

      dataServiceContext.AddObject("Products", product);

      await dataServiceContext.SaveChangesAsync();

      product.Name = "OData renamed";
      dataServiceContext.UpdateObject(product);

      await dataServiceContext.SaveChangesAsync();

      dataServiceContext.DeleteObject(product);

      await dataServiceContext.SaveChangesAsync();
    }

    private static async Task Batch_UniqueKeyViolation()
    {
      var dataServiceContext = new ODataServiceContextFactory().Create(url);

      try
      {
        dataServiceContext.AddObject("Authors", new Author() {LastName = new Random().Next(1, 100000).ToString()});
        dataServiceContext.AddObject("Authors", new Author() {LastName = "Asimov"});
        dataServiceContext.AddObject("Authors", new Author() {LastName = new Random().Next(1, 100000).ToString()});

        var dataServiceResponse = await dataServiceContext.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw;
      }
    }

    private static async Task SetLink_ThenRemoveLink_ThenUpdate_ThenDelete_CompoundKey()
    {
      var dataServiceContext = new ODataServiceContextFactory().Create(url);

      var book = new Book() {Id = "Id" + RandomInt(), Title = "Title " + RandomInt()};
      var publisher = new Publisher() {Title = "New Publisher " + RandomInt()};

      dataServiceContext.AddObject("Books", book);
      dataServiceContext.AddObject("Publishers", publisher);

      dataServiceContext.SetLink(book, nameof(Book.Publisher), publisher);

      var dataServiceResponse = await dataServiceContext.SaveChangesAsync(SaveChangesOptions.None);

      dataServiceContext.SetLink(book, nameof(Book.Publisher), null);

      dataServiceResponse = await dataServiceContext.SaveChangesAsync(SaveChangesOptions.None);

      publisher.Title = publisher.Title + " Changed";

      dataServiceContext.UpdateObject(publisher);
      dataServiceResponse = await dataServiceContext.SaveChangesAsync(SaveChangesOptions.None);

      dataServiceContext.DeleteObject(publisher);
      dataServiceResponse = await dataServiceContext.SaveChangesAsync(SaveChangesOptions.None);
    }

    private static async Task AddLink_Then_DeleteLink_CompoundKey()
    {
      var dataServiceContext = new ODataServiceContextFactory().Create(url);

      var book = new Book() {Id = "Id" + RandomInt(), Title = "Title " + RandomInt()};
      var publisher = new Publisher() {Title = "New Publisher " + RandomInt()};
      
      dataServiceContext.AddObject("Publishers", publisher);
      dataServiceContext.AddObject("Books", book);
      dataServiceContext.AddLink(publisher, "Books", book);

      var dataServiceResponse = await dataServiceContext.SaveChangesAsync(SaveChangesOptions.None);

      dataServiceContext.DeleteLink(publisher, "Books", book);

      dataServiceResponse = await dataServiceContext.SaveChangesAsync(SaveChangesOptions.None);
    }

    private static int RandomInt(int maxValue = 1000000)
    {
      return new Random().Next(1, maxValue);
    }
  }
}