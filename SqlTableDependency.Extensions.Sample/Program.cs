using System;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Joker.Notifications;
using Joker.Redis.ConnectionMultiplexers;
using Newtonsoft.Json;
using Sample.Data.Context;
using Sample.Domain.Models;
using SqlTableDependency.Extensions.Notifications;
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
      await redisPublisher.PublishAsync("messages", "hello");
      await redisPublisher.PublishAsync("messages", "hello2");
      await redisPublisher.SetStringAsync("statusVersion", DateTime.Now.ToString(CultureInfo.InvariantCulture));
      var statusVersion = await redisSubscriber.GetStringAsync("statusVersion");
      DateTime statusDateTimeVersion = DateTime.Parse(statusVersion);

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

    private static void TryModelWithTableAttribute(string connectionString)
    {
      using var productsWithTableAttributeProvider =
        new ProductsWithTableAttributeSqlTableDependencyProvider(connectionString, ThreadPoolScheduler.Instance,
          new ConsoleLogger());

      productsWithTableAttributeProvider.SubscribeToEntityChanges();

      IDisposable whenEntityRecordChangesSubscription = productsWithTableAttributeProvider.WhenEntityRecordChanges
        .Subscribe(c => Console.WriteLine($@"{c.ChangeType} - {c.EntityOldValues?.ReNameD} -> {c.Entity.ReNameD}"));

      Console.ReadKey();

      productsWithTableAttributeProvider.Dispose();
      whenEntityRecordChangesSubscription.Dispose();
    }

    private static void AddOrUpdateProduct(string connectionString)
    {
      using var sampleDbContext = new SampleDbContext(connectionString);
      sampleDbContext.Products.AddOrUpdate(new Product { Id = 1, Name = "New Product3" });
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
                                      }, nameof(Product) + "-Changes");

      await redisSubscriber.Subscribe(channelMessage =>
                                      {
                                        var tableDependencyStatus = JsonConvert.DeserializeObject<VersionedTableDependencyStatus>(channelMessage.Message);
                                        Console.WriteLine($"OnNext tableDependencyStatus changed({tableDependencyStatus.TableDependencyStatus})");
                                      }, nameof(Product) + "-Status");

      redisSubscriber.WhenIsConnectedChanges.Subscribe(c => Console.WriteLine($"REDIS is connected: {c}"));

      return redisSubscriber;
    }
  }
}