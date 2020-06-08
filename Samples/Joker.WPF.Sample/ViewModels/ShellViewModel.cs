using System;
using System.Configuration;
using System.Threading.Tasks;
using Joker.Enums;
using Joker.Factories.Schedulers;
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
using Sample.Domain.Models;
using SqlTableDependency.Extensions;

namespace Joker.WPF.Sample.ViewModels
{
  public class ShellViewModel : BindableBase, IDisposable
  {
    private readonly ReactiveListViewModelFactory viewModelsFactory;
    private ISqlTableDependencyProvider<Product> productsChangesProvider;
    
    private EntityChangesViewModel<ProductViewModel> entityChangesViewModel;

    public EntityChangesViewModel<ProductViewModel> EntityChangesViewModel
    {
      get => entityChangesViewModel;
      set
      {
        entityChangesViewModel = value;

        RaisePropertyChanged();
      }
    }

    private DomainEntitiesSubscriber<Product> domainEntitiesSubscriber;

    public ShellViewModel(ISqlTableDependencyProvider<Product> productsChangesProvider, ReactiveListViewModelFactory viewModelsFactory, IDomainEntitiesSubscriber domainEntitiesSubscriber)
    {
      this.viewModelsFactory = viewModelsFactory ?? throw new ArgumentNullException(nameof(viewModelsFactory));
      
      //Comment/Uncomment case 1. or case 2

      //1. Use inversion of control
      EntityChangesViewModel = viewModelsFactory.CreateProductsEntityChangesViewModel();
      domainEntitiesSubscriber.Subscribe();
      
      //****

      //2. User poor mans dependency injection
      //Manually created object examples
      //use SimulateServer if you didn't run SelfHostedODataService
      //Initialize(productsChangesProvider).ToObservable()
      //  .Subscribe();
    }

    private async Task Initialize(ISqlTableDependencyProvider<Product> productsChangesProvider)
    {
      SimulateServer(productsChangesProvider);

      await InitializeClient();
    }

    private void SimulateServer(ISqlTableDependencyProvider<Product> productsChangesProvider)
    {
      this.productsChangesProvider =
        productsChangesProvider ?? throw new ArgumentNullException(nameof(productsChangesProvider));
      productsChangesProvider.SubscribeToEntityChanges();

      var schedulersFactory = new SchedulersFactory();
      var redisUrl = ConfigurationManager.AppSettings["RedisUrl"];

      var redisPublisher = new ProductSqlTableDependencyRedisProvider(productsChangesProvider,
        new RedisPublisher(redisUrl), schedulersFactory.TaskPool);
      redisPublisher.StartPublishing();
    }

    private async Task InitializeClient()
    {
      var schedulersFactory = new PlatformSchedulersFactory();
      var redisUrl = ConfigurationManager.AppSettings["RedisUrl"];

      var reactiveDataWithStatus = ReactiveDataWithStatus<Product>.Instance;

      domainEntitiesSubscriber =
        new DomainEntitiesSubscriber<Product>(new RedisSubscriber(redisUrl), reactiveDataWithStatus, schedulersFactory);
      await domainEntitiesSubscriber.Subscribe();

      var entityChangesViewModelExample = new ProductsEntityChangesViewModel(
        viewModelsFactory, reactiveDataWithStatus, schedulersFactory);
    }

    private static void CreateReactiveProductsViewModel(ViewModelsFactory viewModelsFactory)
    {
      var reactiveData = new ReactiveDataWithStatus<Product>();
      var redisUrl = ConfigurationManager.AppSettings["RedisUrl"];
      var schedulersFactory = new PlatformSchedulersFactory();
      var entitiesSubscriber = new DomainEntitiesSubscriber<Product>(new RedisSubscriber(redisUrl), reactiveData, schedulersFactory);

      var reactiveProductsViewModel = new ReactiveProductsViewModel(reactiveData, viewModelsFactory, schedulersFactory);

      reactiveProductsViewModel.SubscribeToDataChanges();
      
      reactiveProductsViewModel.Dispose();
      using(entitiesSubscriber)
      { }
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