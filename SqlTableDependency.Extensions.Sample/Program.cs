using System;
using System.Configuration;
using System.Reactive.Concurrency;
using SqlTableDependency.Extensions.Sample.Logging;
using SqlTableDependency.Extensions.Sample.SqlTableDependencies;

namespace SqlTableDependency.Extensions.Sample
{
  class Program
  {
    static void Main(string[] args)
    {
      var connectionString = ConfigurationManager.ConnectionStrings["FargoEntities"].ConnectionString;

      using var productsProvider = new ProductsSqlTableDependencyProvider(connectionString, ThreadPoolScheduler.Instance, new ConsoleLogger());

      Console.WriteLine("Trying to connect...");

      productsProvider.SubscribeToEntityChanges();

      Console.WriteLine("Press a key to stop.");
      Console.ReadKey();
    }
  }
}