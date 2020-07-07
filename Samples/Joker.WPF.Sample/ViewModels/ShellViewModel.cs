using System;
using System.Configuration;
using System.Threading.Tasks;
using Joker.Enums;
using Joker.MVVM.ViewModels;
using Joker.Notifications;
using Joker.Reactive;
using Joker.Redis.ConnectionMultiplexers;
using Joker.Redis.Notifications;
using Joker.WPF.Sample.Factories.Schedulers;
using Joker.PubSubUI.Shared.Factories.ViewModels;
using Joker.PubSubUI.Shared.Navigation;
using Joker.PubSubUI.Shared.ViewModels.Products;
using OData.Client;
using Prism.Mvvm;
using Sample.Domain.Models;

namespace Joker.WPF.Sample.ViewModels
{
  public class ShellViewModel : BindableBase, IDisposable
  {
    private readonly ReactiveListViewModelFactory viewModelsFactory;
    
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

    public ShellViewModel(ReactiveListViewModelFactory viewModelsFactory, IDomainEntitiesSubscriber domainEntitiesSubscriber)
    {
      this.viewModelsFactory = viewModelsFactory ?? throw new ArgumentNullException(nameof(viewModelsFactory));
      
      //Comment/Uncomment case 1. or case 2

      //1. Use inversion of control
      EntityChangesViewModel = viewModelsFactory.CreateProductsEntityChangesViewModel();
      domainEntitiesSubscriber.Subscribe();
      
      //****

      //2. User poor mans dependency injection
      //Manually created objects examples
      //Initialize().ToObservable()
      //  .Subscribe();
    }

    private async Task Initialize()
    {
      await InitializeClient();
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
        viewModelsFactory, reactiveDataWithStatus, schedulersFactory, new ODataServiceContextFactory(), new DialogManager());
    }

    private static void CreateReactiveProductsViewModel(IViewModelsFactory viewModelsFactory)
    {
      var reactiveData = new ReactiveDataWithStatus<Product>();
      var redisUrl = ConfigurationManager.AppSettings["RedisUrl"];
      var schedulersFactory = new PlatformSchedulersFactory();
      var entitiesSubscriber = new DomainEntitiesSubscriber<Product>(new RedisSubscriber(redisUrl), reactiveData, schedulersFactory);

      var reactiveProductsViewModel = new ReactiveProductsViewModel(reactiveData, new ODataServiceContextFactory(), viewModelsFactory, schedulersFactory);

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