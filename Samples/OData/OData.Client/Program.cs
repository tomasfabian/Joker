using System;
using System.Threading.Tasks;
using Microsoft.OData.Client;
using ODataIssue1518.Service.Pocos;

namespace ODataIssue1518.Client
{
  class Program
  {
    static async Task Main(string[] args)
    {
      var dataServiceContext = CreateContext();

      var templateInstance = new TemplateInstance
      {
        Name = "Template 1"
      };

      var boxInstance = new BoxInstance();

      await AddLink(dataServiceContext, boxInstance, templateInstance);
      
      Console.WriteLine("--------");
      Console.WriteLine("--------");
      Console.WriteLine("--------");
      Console.WriteLine("--------");
      Console.WriteLine("--------");

      dataServiceContext = CreateContext();

      await AddRelatedObject(dataServiceContext, boxInstance, templateInstance);

      Console.ReadKey();
    }

    private static async Task AddLink(ODataServiceContext dataServiceContext, BoxInstance boxInstance,
      TemplateInstance templateInstance)
    {
      // this makes sure both objects are tracked in the context
      dataServiceContext.AddObject("BoxInstances", boxInstance);
      dataServiceContext.AddObject("TemplateInstances", templateInstance);

      dataServiceContext.AddLink(templateInstance, "BoxInstances",
        boxInstance); //this will create the batch request to call the TemplateController.CreateRef method

      try
      {
        var response = await dataServiceContext.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
    }

    private static async Task AddRelatedObject(ODataServiceContext dataServiceContext, BoxInstance boxInstance,
      TemplateInstance templateInstance)
    {
      dataServiceContext.AddObject("TemplateInstances", templateInstance);
      dataServiceContext.AddRelatedObject(templateInstance, "BoxInstances", boxInstance);

      try
      {
        var response = await dataServiceContext.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
    }

    static ODataServiceContext CreateContext()
    {
      return new ODataServiceContext(new Uri(@"https://localhost:44364/"));
    }
  }
}
