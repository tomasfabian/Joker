using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Joker.Notifications;
using Joker.Redis.ConnectionMultiplexers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Sample.DataCore.EFCore;
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

      using var productsProvider = new ProductsSqlTableDependencyProvider(connectionString, ThreadPoolScheduler.Instance, new ConsoleLogger());

      using var productSqlTableDependencyRedisProvider = new ProductSqlTableDependencyRedisProvider(productsProvider, redisPublisher, ThreadPoolScheduler.Instance)
        .StartPublishing();

      Console.WriteLine("Trying to connect...");

      productsProvider.SubscribeToEntityChanges();

      await AddOrUpdateProduct(connectionString);

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
        .Subscribe(c => Console.WriteLine($"{c.ChangeType} - {c.EntityOldValues?.ReNameD} -> {c.Entity.ReNameD}"));

      Console.ReadKey();

      whenEntityRecordChangesSubscription.Dispose();
    }

    private static async Task AddOrUpdateProduct(string connectionString)
    {
      var optionsBuilder = new DbContextOptionsBuilder<SampleDbContextCore>();
      optionsBuilder.UseSqlServer(connectionString);

      await using var sampleDbContext = new SampleDbContextCore(optionsBuilder.Options);

      int id = 1;
      var product = await sampleDbContext.Products.FirstOrDefaultAsync(c => c.Id == id);
      if (product == null)
        sampleDbContext.Products.Add(new Product { Name = "New Product" });
      else
      {
        product.Name = "New Product - changed";
        sampleDbContext.Products.Update(product);
      }

      await sampleDbContext.SaveChangesAsync();

      var products = sampleDbContext.Products.AsNoTracking().ToList();
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
    private static async Task DeleteAllAsync(string connectionString)
    {
      var optionsBuilder = new DbContextOptionsBuilder<SampleDbContextCore>();
      optionsBuilder.UseSqlServer(connectionString);

      await using var dbContext = new SampleDbContextCore(optionsBuilder.Options);

      dbContext.Authors.RemoveRange(dbContext.Authors);
      dbContext.Books.RemoveRange(dbContext.Books);
      dbContext.Publishers.RemoveRange(dbContext.Publishers);

      int result = await dbContext.SaveChangesAsync();
    }
  }
}