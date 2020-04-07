using System;
using System.Configuration;
using System.Reactive.Linq;
using Joker.Enums;
using Joker.MVVM.ViewModels;
using Joker.Reactive;
using Joker.Redis.ConnectionMultiplexers;
using Joker.Redis.Notifications;
using Joker.WPF.Sample.Factories.Schedulers;
using Joker.WPF.Sample.Factories.ViewModels;
using Joker.WPF.Sample.Redis;
using Joker.WPF.Sample.ViewModels.Products;
using Joker.WPF.Sample.ViewModels.Reactive;
using Prism.Mvvm;
using Sample.Data.Context;
using Sample.Domain.Models;
using SqlTableDependency.Extensions;

namespace Joker.WPF.Sample.ViewModels
{
  public class ShellViewModel : BindableBase, IDisposable
  {
    private readonly ISqlTableDependencyProvider<Product> productsChangesProvider;

    public EntityChangesViewModel<ProductViewModel> EntityChangesViewModel { get; }

    private readonly DomainEntitiesSubscriber<Product> domainEntitiesSubscriber;

    public ShellViewModel(ProductsViewModel productsViewModel, ISqlTableDependencyProvider<Product> productsChangesProvider)
    {
      this.productsChangesProvider = productsChangesProvider ?? throw new ArgumentNullException(nameof(productsChangesProvider));
      productsChangesProvider.SubscribeToEntityChanges();
      
      var schedulersFactory = new WpfSchedulersFactory();
      var redisUrl = ConfigurationManager.AppSettings["RedisUrl"];

      var redisPublisher = new ProductSqlTableDependencyRedisProvider(productsChangesProvider,
        new RedisPublisher(redisUrl), schedulersFactory.TaskPool);
      redisPublisher.StartPublishing();

      var reactiveDataWithStatus = new ReactiveDataWithStatus<Product>();
      
      domainEntitiesSubscriber = new DomainEntitiesSubscriber<Product>(new RedisSubscriber(redisUrl), reactiveDataWithStatus, schedulersFactory);
      domainEntitiesSubscriber.Subscribe();

      EntityChangesViewModel = new EntityChangesViewModel<ProductViewModel>(
        new ReactiveListViewModelFactory() {ReactiveDataWithStatus = reactiveDataWithStatus}, reactiveDataWithStatus, schedulersFactory.Dispatcher);
    }

    private static void CreateReactiveProductsViewModel()
    {
      var reactiveData = new ReactiveDataWithStatus<Product>();
      var redisUrl = ConfigurationManager.AppSettings["RedisUrl"];
      var schedulersFactory = new WpfSchedulersFactory();
      using var entitiesSubscriber = new DomainEntitiesSubscriber<Product>(new RedisSubscriber(redisUrl), reactiveData, schedulersFactory);

      string connectionString = ConfigurationManager.ConnectionStrings["FargoEntities"].ConnectionString;

      var reactiveProductsViewModel = new ReactiveProductsViewModel(new SampleDbContext(connectionString),
        reactiveData, schedulersFactory);

      reactiveProductsViewModel.SubscribeToDataChanges();
      
      reactiveProductsViewModel.Dispose();
    }

    private static int fakeProductId = 1;

    private static EntityChange<Product> CreateEntityChange()
    {
      return new EntityChange<Product>(new Product()
      {
        Id = ++fakeProductId, 
        Timestamp = DateTime.Now,
        Name = $"Product {fakeProductId}"
      }, ChangeType.Create);
    }

    public void Dispose()
    {
      using (EntityChangesViewModel)
      using (domainEntitiesSubscriber)
      {
      }
    }
  }
}