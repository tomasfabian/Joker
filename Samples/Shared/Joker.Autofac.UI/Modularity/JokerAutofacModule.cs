using Autofac;
using Joker.BlazorApp.Sample.Factories.ViewModels;
using Joker.BlazorApp.Sample.Subscribers;
using Joker.Contracts;
using Joker.Factories.Schedulers;
using Joker.MVVM.ViewModels;
using Joker.Notifications;
using Joker.Platforms.Factories.Schedulers;
using Joker.PubSubUI.Shared.Factories.ViewModels;
using Joker.PubSubUI.Shared.Navigation;
using Joker.PubSubUI.Shared.ViewModels.Products;
using Joker.Reactive;
using Microsoft.OData.Edm;
using OData.Client;
using Sample.Domain.Models;

namespace Joker.Autofac.UI.Modularity
{
  public class JokerAutofacModule : Module
  {
    protected override void Load(ContainerBuilder containerBuilder)
    {
      base.Load(containerBuilder);

      containerBuilder.RegisterType<ODataServiceContextFactory>()
        .As<IODataServiceContextFactory>()
        .SingleInstance();

      //containerBuilder.Register(c => serviceModel)
      //  .As<IEdmModel>()
      //  .SingleInstance();

      containerBuilder.RegisterType<ReactiveListViewModelFactory>()
        .As<IReactiveListViewModelFactory<ProductViewModel>>()
        .SingleInstance();

      //containerBuilder.RegisterType<PlatformSchedulersFactory>()
      //  .As<IPlatformSchedulersFactory, ISchedulersFactory>()
      //  .SingleInstance();

      containerBuilder.RegisterType<DialogManager>()
        .As</*IBlazorDialogManager, */IDialogManager>()
        .SingleInstance();

      containerBuilder.Register(c => ReactiveDataWithStatus<Product>.Instance)
        .As<IReactiveData<Product>, IEntityChangePublisherWithStatus<Product>, ITableDependencyStatusProvider>()
        .SingleInstance();

      containerBuilder.RegisterType<ProductsEntityChangesViewModel>().AsSelf();
      containerBuilder.RegisterType<ReactiveProductsViewModel>().AsSelf();
      containerBuilder.RegisterType<ProductViewModel>().AsSelf();
      containerBuilder.RegisterType<ViewModelsFactory>().As<IViewModelsFactory>();

      containerBuilder.RegisterType<DomainEntitiesSubscriber<Product>>()
        .As<IDomainEntitiesSubscriber>();
    }
  }
}