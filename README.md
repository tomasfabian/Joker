# SqlTableDependency.Extensions

## See:
https://github.com/christiandelbianco/monitor-table-change-with-sqltabledependency

Following package is based on christiandelbianco's SqlTableDependency. SqlTableDependencyProvider provides periodic reconnection in case of any error like lost connection etc.

## Basic usage:
```C#
  public class Product
  {
      public int Id { get; set; }
      public string Name { get; set; }
  }   
    
  internal class ProductsSqlTableDependencyProvider : SqlTableDependencyProvider<Product>
  {
    private readonly ILogger logger;

    internal ProductsSqlTableDependencyProvider(ConnectionStringSettings connectionStringSettings, IScheduler scheduler, ILogger logger)
      : base(connectionStringSettings, scheduler)
    {
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    internal ProductsSqlTableDependencyProvider(string connectionString, IScheduler scheduler, ILogger logger)
      : base(connectionString, scheduler)
    {
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override ModelToTableMapper<Product> OnInitializeMapper(ModelToTableMapper<Product> modelToTableMapper)
    {
      modelToTableMapper.AddMapping(c => c.Id, nameof(Product.Id));

      return modelToTableMapper;
    }

    protected override void OnInserted(Product product)
    {
      base.OnInserted(product);

      logger.Trace("Entity was added");

      LogChangeInfo(product);
    }

    protected override void OnUpdated(Product product)
    {
      base.OnUpdated(product);

      logger.Trace("Entity was modified");

      LogChangeInfo(product);
    }

    protected override void OnDeleted(Product product)
    {
      base.OnDeleted(product);

      logger.Trace("Entity was deleted");

      LogChangeInfo(product);
    }

    private void LogChangeInfo(Product product)
    {
      Console.WriteLine(Environment.NewLine);

      Console.WriteLine("Id: " + product.Id);
      Console.WriteLine("Name: " + product.Name);

      Console.WriteLine("#####");
      Console.WriteLine(Environment.NewLine);
    }
  }
```
//Program.cs
```C#
using System.Configuration;
using System.Reactive.Concurrency;

namespace SqlTableDependency.Extensions.Sample
{
  class Program
  {
    static void Main(string[] args)
    {
      var connectionString = ConfigurationManager.ConnectionStrings["FargoEntities"].ConnectionString;
      
      using var productsProvider = new ProductsSqlTableDependencyProvider(connectionString, ThreadPoolScheduler.Instance, new ConsoleLogger());
      productsProvider.SubscribeToEntityChanges();
      
      Console.ReadKey();
    }
  }
}
