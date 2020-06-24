using System.Configuration;
using Joker.Contracts;
using Joker.MVVM.ViewModels;
using Joker.Reactive;
using Joker.Redis.ConnectionMultiplexers;
using Joker.Redis.Notifications;
using Joker.PubSubUI.Shared.Factories.ViewModels;
using Joker.PubSubUI.Shared.ViewModels.Products;
using Ninject.Modules;
using OData.Client;
using Sample.Domain.Models;

namespace Joker.WPF.Sample.Modularity
{
  public class AppNinjectModule : NinjectModule
  {
    public override void Load()
    {
      Bind<IViewModelsFactory>().To<ViewModelsFactory>().InSingletonScope();
      Bind<IODataServiceContextFactory>().To<ODataServiceContextFactory>().InSingletonScope();

      Bind<IReactiveListViewModelFactory<ProductViewModel>, ReactiveListViewModelFactory>().To<ReactiveListViewModelFactory>().InSingletonScope();

      Bind<ITableDependencyStatusProvider, IReactiveData<Product>, IEntityChangePublisherWithStatus<Product>, ReactiveDataWithStatus<Product>>()
        .ToConstant(ReactiveDataWithStatus<Product>.Instance);
      
      var redisUrl = ConfigurationManager.AppSettings["RedisUrl"];

      Bind<IRedisSubscriber>().To<RedisSubscriber>()
        .WithConstructorArgument("url", redisUrl);

      Bind<IDomainEntitiesSubscriber>().To<DomainEntitiesSubscriber<Product>>();
    }
  }
}