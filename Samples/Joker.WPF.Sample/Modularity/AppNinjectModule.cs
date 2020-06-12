using System.Configuration;
using Joker.Contracts;
using Joker.Factories.Schedulers;
using Joker.MVVM.ViewModels;
using Joker.Platforms.Factories.Schedulers;
using Joker.PubSubUI.Shared.Navigation;
using Joker.Reactive;
using Joker.Redis.ConnectionMultiplexers;
using Joker.Redis.Notifications;
using Joker.WPF.Sample.Factories.Schedulers;
using Joker.PubSubUI.Shared.Factories.ViewModels;
using Joker.WPF.Sample.Navigation;
using Joker.PubSubUI.Shared.ViewModels.Products;
using Ninject.Modules;
using Sample.Domain.Models;

namespace Joker.WPF.Sample.Modularity
{
  public class AppNinjectModule : NinjectModule
  {
    public override void Load()
    {
      Bind<IDialogManager>().To<DialogManager>().InSingletonScope();

      Bind<ISchedulersFactory, IPlatformSchedulersFactory>().To<PlatformSchedulersFactory>().InSingletonScope();
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