using System;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sample.Data.Context;
using Sample.Domain.Models;
using SqlTableDependency.Extensions.Notifications;
using SqlTableDependency.Extensions.Redis.ConnectionMultiplexers;
using SqlTableDependency.Extensions.Redis.SqlTableDependency;
using SqlTableDependency.Extensions.Sample.Logging;
using SqlTableDependency.Extensions.Sample.Redis;
using SqlTableDependency.Extensions.Sample.SqlTableDependencies;
using TableDependency.SqlClient.Base.Enums;

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
      await redisPublisher.PublishAsync("messages", "hello");

      using var productsProvider = new ProductsSqlTableDependencyProvider(connectionString, ThreadPoolScheduler.Instance, new ConsoleLogger());

      using var productSqlTableDependencyRedisProvider = new ProductSqlTableDependencyRedisProvider(productsProvider, redisPublisher, ThreadPoolScheduler.Instance)
        .StartPublishing();

      Console.WriteLine("Trying to connect...");

      productsProvider.SubscribeToEntityChanges();

      AddOrUpdateProduct(connectionString);

      Console.WriteLine("Press a key to stop.");
      Console.ReadKey();

      redisSubscriber.Unsubscribe(nameof(Product) + "-Changes");
      redisSubscriber.Unsubscribe(nameof(Product) + "-Status");

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
      
      await redisSubscriber.Subscribe(channelMessage =>
                                      {
                                        var recordChange = JsonConvert.DeserializeObject<RecordChangedNotification<Product>>(channelMessage.Message);
                                        Console.WriteLine($"OnNext Product changed({recordChange.ChangeType})");
                                        Console.WriteLine($"OnNext Product changed({recordChange.Entity.Id})");
                                      }, nameof(Product)+"-Changes");      
      
      await redisSubscriber.Subscribe(channelMessage =>
                                      {
                                        var tableDependencyStatus = JsonConvert.DeserializeObject<TableDependencyStatus>(channelMessage.Message);
                                        Console.WriteLine($"OnNext tableDependencyStatus changed({tableDependencyStatus})");
                                      }, nameof(Product)+"-Status");

      redisSubscriber.WhenIsConnectedChanges.Subscribe(c => Console.WriteLine($"REDIS is connected: {c}"));
      return redisSubscriber;
    }
  }
}