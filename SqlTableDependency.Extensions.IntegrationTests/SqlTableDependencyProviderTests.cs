using System;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample.Data.Context;
using Sample.Domain.Models;
using SqlTableDependency.Extensions.Enums;
using SqlTableDependency.Extensions.Providers.Sql;
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
    public static void ClassInitialize(TestContext testContext)
    {
      tableDependencyProvider = new ProductsSqlTableDependencyProvider(ConnectionString, ThreadPoolScheduler.Instance, LifetimeScope.UniqueScope);

      tableDependencyProvider.SubscribeToEntityChanges();
    }

    public const string IntegrationTests = "IntegrationTests";

    [TestMethod]
    [TestCategory(IntegrationTests)]
    public async Task SubscribeToEntityChanges()
    {      
      await OnInserted();

      await OnUpdated();

      await OnDeleted();
    }

    public async Task OnInserted()
    {
      var product = new Product
      {
        Name = "New Product3",
        Timestamp = DateTime.Now
      };

      AddOrUpdateProduct(product);

      await tableDependencyProvider.LastInsertedProductChanged.WaitFirst();

      tableDependencyProvider.LastInsertedProduct.Should().NotBeNull();
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

    [ClassCleanup]
    public static void ClassCleanup()
    {
      using (tableDependencyProvider)
      {
        tableDependencyProvider = null;
      }
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