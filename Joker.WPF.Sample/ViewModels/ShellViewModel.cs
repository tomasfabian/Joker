using System;
using System.Configuration;
using System.Reactive.Linq;
using Joker.Enums;
using Joker.Factories.Schedulers;
using Joker.Reactive;
using Joker.Redis.ConnectionMultiplexers;
using Joker.Redis.Notifications;
using Joker.WPF.Sample.Factories.Schedulers;
using Joker.WPF.Sample.ViewModels.Products;
using Joker.WPF.Sample.ViewModels.Reactive;
using Prism.Mvvm;
using Sample.Data.Context;
using Sample.Domain.Models;

namespace Joker.WPF.Sample.ViewModels
{
  public class ShellViewModel : BindableBase, IDisposable
  {
    public ProductsViewModel ProductsViewModel { get; }

    public ReactiveProductsViewModel ReactiveProductsViewModel { get; }

    private static int fakeProductId = 1;

    private readonly IDisposable timerSubscription;
    private readonly IDisposable isLoadingSubscription;

    public ShellViewModel(ProductsViewModel productsViewModel)
    {
      ProductsViewModel = productsViewModel;

      // ProductsViewModel.Initialize();

      var reactiveData = new ReactiveData<Product>();
      
      ReactiveProductsViewModel = new ReactiveProductsViewModel(null, reactiveData, new WpfSchedulersFactory());

      ReactiveProductsViewModel.SubscribeToDataChanges();

      reactiveData.Publish(CreateEntityChange());

      isLoadingSubscription = ReactiveProductsViewModel.IsLoadingChanged
        .Subscribe(c =>
        {
          Console.WriteLine($"IsLoading changed {c}");
        });

      timerSubscription = Observable.Timer(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1))
        .Subscribe(c =>
        {
          reactiveData.Publish(CreateEntityChange());
        });

      var reactiveDataWithStatus = new ReactiveDataWithStatus<Product>();
      var redisUrl = ConfigurationManager.AppSettings["RedisUrl"];
      using var entitiesSubscriber = new DomainEntitiesSubscriber<Product>(new RedisSubscriber(redisUrl), reactiveDataWithStatus, new SchedulersFactory());
      reactiveDataWithStatus.WhenStatusChanges
        .Subscribe(c =>
        {

        });
      entitiesSubscriber.Subscribe();
      //CreateReactiveProductsViewModel();
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
      using (isLoadingSubscription)
      using (timerSubscription)
      {
      }

      ProductsViewModel?.Dispose();
    }
  }
}