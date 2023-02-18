using System;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using FluentAssertions;
using Joker.Extensions;
using Joker.Factories.Schedulers;
using Joker.Redis.ConnectionMultiplexers;
using Joker.Redis.Notifications;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample.Data.Context;
using Sample.Domain.Models;
using SqlTableDependency.Extensions.Enums;
using SqlTableDependency.Extensions.Providers.Sql;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Exceptions;
using UnitTests;
using ChangeType = Joker.Enums.ChangeType;

namespace SqlTableDependency.Extensions.IntegrationTests
{
  [TestClass]
  public class SqlTableDependencyProviderTests : TestBase
  {
    private static readonly string RedisUrl = ConfigurationManager.AppSettings["RedisUrl"];
    private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["SampleDbContext"].ConnectionString;

    private static ProductsSqlTableDependencyProvider tableDependencyProvider;

    [ClassInitialize]
    public static async Task ClassInitialize(TestContext testContext)
    {
      ProcessProvider.Docker("start sql");

      await new SqlConnectionProvider().EnableServiceBroker(ConnectionString);

      new SqlConnectionProvider().IsServiceBrokerEnabled(ConnectionString).Should().BeTrue();
    }

    public const string IntegrationTests = "IntegrationTests";
    
    [TestMethod]
    [TestCategory(IntegrationTests)]
    public async Task KillSessions_UniqueScope_SqlTableDependencyProviderReconnects()
    {
      await ReconnectionTest(LifetimeScope.UniqueScope);
    }   

    [TestMethod]
    [TestCategory(IntegrationTests)]
    public async Task KillSessions_ApplicationScope_SqlTableDependencyProviderReconnects()
    {
      await ReconnectionTest(LifetimeScope.ApplicationScope);
    }

    private static async Task ReconnectionTest(LifetimeScope lifetimeScope)
    {
      tableDependencyProvider =
        new ProductsSqlTableDependencyProvider(ConnectionString, TaskPoolScheduler.Default, lifetimeScope);

      tableDependencyProvider.SubscribeToEntityChanges();
      
      Product product = null;

      bool isFirstStart = true;
      var subscription =
        tableDependencyProvider.WhenStatusChanges
          .Subscribe(c =>
          {
            if (c.IsOneOfFollowing(TableDependencyStatus.Started, TableDependencyStatus.WaitingForNotification) &&
                isFirstStart)
            {
              isFirstStart = false;

              //ProcessProvider.Docker("stop sql");

              SqlConnectionProvider.KillSessions(ConnectionString);
              
              product = InsertNewProduct();
            }
          });

      await tableDependencyProvider.LastExceptionChanged.Select(c => Unit.Default)
        .Merge(tableDependencyProvider.LastInsertedProductChanged.Select(c => Unit.Default))
        .WaitFirst(TimeSpan.FromSeconds(10));
      
      // ProcessProvider.Docker("start sql");
      
      await tableDependencyProvider.WhenStatusChanges.Where(c =>
          c.IsOneOfFollowing(TableDependencyStatus.Started, TableDependencyStatus.WaitingForNotification))
        .FirstOrDefaultAsync().ToTask();
      
      using (subscription)
      {
      }

      DeleteProduct(product);

      await tableDependencyProvider.LastDeletedProductChanged
        .Where(c => c.Id == product.Id)
        .WaitFirst(tableDependencyProvider.ReconnectionTimeSpan);

      tableDependencyProvider.LastInsertedProduct.Should().NotBeNull();
    }

    [TestMethod]
    [TestCategory(IntegrationTests)]
    [DataRow(LifetimeScope.ApplicationScope)]
    [DataRow(LifetimeScope.ConnectionScope)]
    [DataRow(LifetimeScope.UniqueScope)]
    public async Task SubscribeToEntityChanges(LifetimeScope lifetimeScope)
    {
      tableDependencyProvider = new ProductsSqlTableDependencyProvider(ConnectionString, ThreadPoolScheduler.Instance, lifetimeScope);

      tableDependencyProvider.SubscribeToEntityChanges();

      await OnInserted();

      await OnUpdated();

      await OnDeleted();
    }

    public async Task OnInserted()
    {
      var product = InsertNewProduct();

      await tableDependencyProvider.LastInsertedProductChanged.Where(c => c.Id == product.Id).WaitFirst();

      tableDependencyProvider.LastInsertedProduct.Should().NotBeNull();
    }

    private static Product InsertNewProduct()
    {
      var product = new Product
      {
        Name = "New Product3",
        Timestamp = DateTime.Now
      };

      return AddOrUpdateProduct(product);
    }

    public async Task OnUpdated()
    {
      var product = tableDependencyProvider.LastInsertedProduct;
      
      product.Name = "Updated";

      AddOrUpdateProduct(product);

      await tableDependencyProvider.LastUpdatedProductChanged.WaitFirst();

      tableDependencyProvider.LastUpdatedProduct.Should().NotBeNull();
    }

    public async Task OnDeleted()
    {
      DeleteProduct(tableDependencyProvider.LastInsertedProduct);

      await tableDependencyProvider.LastDeletedProductChanged.WaitFirst();

      tableDependencyProvider.LastDeletedProduct.Should().NotBeNull();
    }

    [TestCleanup]
    public override void TestCleanup()
    {
      base.TestCleanup();

      using (tableDependencyProvider)
      {
        tableDependencyProvider = null;
      }
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      //new SqlConnectionProvider().DisableServiceBroker(ConnectionString).Wait();

      //new SqlConnectionProvider().IsServiceBrokerEnabled(ConnectionString).Should().BeFalse();
    }

    private static Product AddOrUpdateProduct(Product product)
    {
      try
      {
        using var sampleDbContext = new SampleDbContext(ConnectionString);

        sampleDbContext.Products.AddOrUpdate(product);
        sampleDbContext.SaveChanges();

        // var products = sampleDbContext.Products.ToList();

        return product;
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw;
      }
    }

    private static void DeleteProduct(Product product)
    {
      using var sampleDbContext = new SampleDbContext(ConnectionString);

      sampleDbContext.Products.Attach(product);
      sampleDbContext.Products.Remove(product);

      sampleDbContext.SaveChanges();
    }

    private async Task TestAlterServiceBroker()
    {
      await new SqlConnectionProvider().DisableServiceBroker(ConnectionString);

      await new SqlConnectionProvider().EnableServiceBroker(ConnectionString);
    }

    [TestMethod]
    public async Task TestRedisPubSub()
    {      
      tableDependencyProvider =
        new ProductsSqlTableDependencyProvider(ConnectionString, TaskPoolScheduler.Default, LifetimeScope.UniqueScope);

      tableDependencyProvider.SubscribeToEntityChanges();

      var schedulersFactory = new SchedulersFactory();

      using var redisPublisher = new ProductSqlTableDependencyRedisProvider(tableDependencyProvider,
        new RedisPublisher(RedisUrl), schedulersFactory.TaskPool);
      
      redisPublisher.StartPublishing();

      var reactiveDataWithStatus = new TestableReactiveData<Product>();

      using var domainEntitiesSubscriber =
        new DomainEntitiesSubscriber<Product>(new RedisSubscriber(RedisUrl), reactiveDataWithStatus, schedulersFactory);

      await domainEntitiesSubscriber.Subscribe();

      var product = InsertNewProduct();

      var productEntityChange = await reactiveDataWithStatus.WhenDataChanges
        .Where(c => c.Entity.Id == product.Id)
        .FirstOrDefaultAsync()
        .ToTask();

      productEntityChange.ChangeType.Should().Be(ChangeType.Create);

      product.Name = "Updated";
      AddOrUpdateProduct(product);

      var productUpdated = await reactiveDataWithStatus.WhenDataChanges
        .Where(c => c.Entity.Id == product.Id && c.ChangeType == ChangeType.Update)
        .FirstOrDefaultAsync()
        .ToTask();

      productUpdated.ChangeType.Should().Be(ChangeType.Update);

      DeleteProduct(product);

      var productDeleted = await reactiveDataWithStatus.WhenDataChanges
        .Where(c => c.Entity.Id == product.Id && c.ChangeType == ChangeType.Delete)
        .FirstOrDefaultAsync()
        .ToTask();

      productDeleted.ChangeType.Should().Be(ChangeType.Delete);
    }

    [TestMethod]
    [ExpectedException(typeof(ServiceBrokerNotEnabledException))]
    [Ignore]
    public void ServiceBrokerNotEnabledException()
    {
      //TODO
    }
  }
}