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

namespace SqlTableDependency.Extensions.Sample
{
  class Program
  {
    static async Task Main(string[] args)
    {
      string redisUrl = ConfigurationManager.AppSettings["RedisUrl"];
      var connectionString = ConfigurationManager.ConnectionStrings["FargoEntities"].ConnectionString;

      using var redisSubscriber = await CreateRedisSubscriber(redisUrl);

      var redisPublisher = new RedisPublisher(redisUrl);
      await redisPublisher.Publish("messages", "hello");

      using var productsProvider = new ProductsSqlTableDependencyProvider(connectionString, ThreadPoolScheduler.Instance, new ConsoleLogger());

      using var productSqlTableDependencyRedisProvider = new ProductSqlTableDependencyRedisProvider(productsProvider, redisPublisher)
        .StartPublishing();

      Console.WriteLine("Trying to connect...");

      productsProvider.SubscribeToEntityChanges();

      AddOrUpdateProduct(connectionString);

      Console.WriteLine("Press a key to stop.");
      Console.ReadKey();

      redisSubscriber.Dispose();
      redisPublisher.Dispose();
    }

    private static void AddOrUpdateProduct(string connectionString)
    {
      using var sampleDbContext = new SampleDbContext(connectionString);
      sampleDbContext.Products.AddOrUpdate(new Product {Id = 1, Name = "New Product3"});
      sampleDbContext.SaveChanges();

      var products = sampleDbContext.Products.ToList();
    }

    private static async Task<RedisSubscriber> CreateRedisSubscriber(string redisUrl)
    {
      var redisSubscriber = new RedisSubscriber(redisUrl);

      await redisSubscriber.Subscribe(channelMessage => { Console.WriteLine($"OnNext({channelMessage.Message})"); },
        "messages");

      redisSubscriber.WhenIsConnectedChanges.Subscribe(c => Console.WriteLine($"REDIS is connected: {c}"));
      return redisSubscriber;
    }
  }
}