using System;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Sample.Data.Context;
using Sample.Domain.Models;
using SqlTableDependency.Extensions.Redis.ConnectionMultiplexers;
using SqlTableDependency.Extensions.Sample.Logging;
using SqlTableDependency.Extensions.Sample.Redis;
using SqlTableDependency.Extensions.Sample.SqlTableDependencies;
using StackExchange.Redis;

namespace SqlTableDependency.Extensions.Sample
{
  class Program
  {
    static async Task Main(string[] args)
    {
      string redisUrl = "localhost";
      
      var redisSubscriber = new RedisSubscriber(redisUrl);

      await redisSubscriber.Subscribe(channelMessage => {
                                        Console.WriteLine(channelMessage.Message);
                                      }, "messages");

      var redisPublisher = new RedisPublisher(redisUrl);
      await redisPublisher.Publish("messages", "hello");

      var connectionString = ConfigurationManager.ConnectionStrings["FargoEntities"].ConnectionString;
      
      using var sampleDbContext2 = new SampleDbContext(connectionString);
      var products2 = sampleDbContext2.Products.ToList();

      using var productsProvider = new ProductsSqlTableDependencyProvider(connectionString, ThreadPoolScheduler.Instance, new ConsoleLogger());

      using var productSqlTableDependencyRedisProvider = new ProductSqlTableDependencyRedisProvider(productsProvider, redisPublisher);

      Console.WriteLine("Trying to connect...");

      productsProvider.SubscribeToEntityChanges();

      using var sampleDbContext = new SampleDbContext(connectionString);
      sampleDbContext.Products.AddOrUpdate(new Product { Id = 1, Name = "New Product3" });
      sampleDbContext.SaveChanges();
      var products = sampleDbContext.Products.ToList();

      Console.WriteLine("Press a key to stop.");
      Console.ReadKey();

      redisSubscriber.Dispose();
      redisPublisher.Dispose();
    }
  }
}