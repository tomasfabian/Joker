using Autofac;
using Autofac.Extensions.DependencyInjection;
using Joker.Contracts;
using Joker.MVVM.ViewModels;
using Joker.Platforms.Factories.Schedulers;
using Joker.PubSubUI.Shared.Factories.ViewModels;
using Joker.PubSubUI.Shared.ViewModels.Products;
using Joker.Reactive;
using Joker.Redis.ConnectionMultiplexers;
using Joker.Redis.Notifications;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sample.Domain.Models;
using System;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Autofac.Core;
using Joker.BlazorApp.Sample.Factories.Schedulers;
using Joker.Factories.Schedulers;
using Joker.PubSubUI.Shared.Navigation;
using ReactiveListViewModelFactory = Joker.BlazorApp.Sample.Factories.ViewModels.ReactiveListViewModelFactory;
using ViewModelsFactory = Joker.BlazorApp.Sample.Factories.ViewModels.ViewModelsFactory;

namespace Joker.BlazorApp.Sample
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      var builder = WebAssemblyHostBuilder.CreateDefault(args);
     
      builder.ConfigureContainer(new AutofacServiceProviderFactory(), containerBuilder =>
      {      
        containerBuilder.RegisterType<Service>()
          .SingleInstance();

        containerBuilder.RegisterType<ReactiveListViewModelFactory>()
          .As<IReactiveListViewModelFactory<ProductViewModel>>()
          .SingleInstance();

        containerBuilder.RegisterType<PlatformSchedulersFactory>()
          .As<IPlatformSchedulersFactory, ISchedulersFactory>()
          .SingleInstance();

        containerBuilder.RegisterType<DialogManager>()
          .As<IDialogManager>()
          .SingleInstance();

        containerBuilder.RegisterType<ReactiveDataWithStatus<Product>>()
          .As<ITableDependencyStatusProvider, IReactiveData<Product>, IEntityChangePublisherWithStatus<Product>>()
          .SingleInstance();        
        
        containerBuilder.RegisterType<ProductsEntityChangesViewModel>().AsSelf();
        containerBuilder.RegisterType<ReactiveProductsViewModel>().AsSelf();
        containerBuilder.RegisterType<ViewModelsFactory>().As<IViewModelsFactory>();

        var baseAddress = builder.Configuration.GetValue<string>("BaseUrl");

        containerBuilder.RegisterType<RedisSubscriber>()
          .As<IRedisSubscriber>()
          .WithParameter("url", baseAddress)
          .InstancePerLifetimeScope();

        containerBuilder.RegisterType<DomainEntitiesSubscriber<Product>>()
          .As<IDomainEntitiesSubscriber>();
      }); 

      builder.RootComponents.Add<App>("app");
      
      builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
      
      await builder.Build().RunAsync();
    }
  }

  public class DialogManager : IDialogManager
  {
    public void ShowMessage(string message)
    {
    }
  }
}