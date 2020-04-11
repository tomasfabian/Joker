using System;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using FluentAssertions;
using Joker.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample.Data.Context;
using Sample.Domain.Models;
using SqlTableDependency.Extensions.Enums;
using SqlTableDependency.Extensions.Providers.Sql;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Exceptions;
using UnitTests;

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
      ProcessProvider.Docker($"start sql");

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

    private static async Task ReconnectionTest(LifetimeScope lifetimeScope)
    {
      tableDependencyProvider =
        new ProductsSqlTableDependencyProvider(ConnectionString, TaskPoolScheduler.Default, lifetimeScope);

      tableDependencyProvider.SubscribeToEntityChanges();

      bool isFirstStart = true;
      var subscription =
        tableDependencyProvider.WhenStatusChanges
          .Subscribe(c =>
          {
            if (c.IsOneOfFollowing(TableDependencyStatus.Started, TableDependencyStatus.WaitingForNotification) &&
                isFirstStart)
            {
              isFirstStart = false;
              //Docker($"stop sql");
              //Docker($"start sql");
              SqlConnectionProvider.KillSessions(ConnectionString);
            }
          });
      
      using (subscription)
      {
      }

      await tableDependencyProvider.WhenStatusChanges.Where(c =>
          c.IsOneOfFollowing(TableDependencyStatus.Started, TableDependencyStatus.WaitingForNotification))
        .FirstOrDefaultAsync().ToTask();
      
      Product product = InsertNewProduct();

      await tableDependencyProvider.LastInsertedProductChanged.WaitFirst(tableDependencyProvider.ReconnectionTimeSpan);

      tableDependencyProvider.LastInsertedProduct.Should().NotBeNull();

      DeleteProduct(product);
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
      new SqlConnectionProvider().DisableServiceBroker(ConnectionString).Wait();

      new SqlConnectionProvider().IsServiceBrokerEnabled(ConnectionString).Should().BeFalse();
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
    [ExpectedException(typeof(ServiceBrokerNotEnabledException))]
    [Ignore]
    public void ServiceBrokerNotEnabledException()
    {
    }
  }
}