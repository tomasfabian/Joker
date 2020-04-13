using System.Configuration;
using System.Threading.Tasks;
using System.Windows;
using CommonServiceLocator;
using Joker.Contracts;
using Joker.Factories.Schedulers;
using Joker.MVVM.ViewModels;
using Joker.Reactive;
using Joker.Redis.ConnectionMultiplexers;
using Joker.Redis.Notifications;
using Joker.WPF.Sample.Factories.Schedulers;
using Joker.WPF.Sample.Factories.ViewModels;
using Joker.WPF.Sample.Modularity;
using Joker.WPF.Sample.Providers.Scheduling;
using Joker.WPF.Sample.SqlTableDependencies;
using Joker.WPF.Sample.ViewModels;
using Joker.WPF.Sample.ViewModels.Products;
using Joker.WPF.Sample.ViewModels.Reactive;
using Ninject;
using Prism.Ioc;
using Prism.Ninject.Ioc;
using Sample.Data.Context;
using Sample.Domain.Models;
using SqlTableDependency.Extensions;
using SqlTableDependency.Extensions.Enums;

namespace Joker.WPF.Sample
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App
  {
    private readonly IKernel kernel = new StandardKernel();

    #region CreateContainerExtension

    protected override IContainerExtension CreateContainerExtension()
    {
      return new NinjectContainerExtension(kernel);
    }

    #endregion

    #region RegisterTypes

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
      kernel.Bind<ShellViewModel>().ToSelf().InSingletonScope();

      var connectionString = ConfigurationManager.ConnectionStrings["FargoEntities"].ConnectionString;
      var redisUrl = ConfigurationManager.AppSettings["RedisUrl"];

      kernel.Bind<ISchedulerProvider>().To<SchedulerProvider>().InSingletonScope();
      kernel.Bind<IReactiveListViewModelFactory<ProductViewModel>, ReactiveListViewModelFactory>().To<ReactiveListViewModelFactory>().InSingletonScope();
      kernel.Bind<ISchedulersFactory, IWpfSchedulersFactory>().To<WpfSchedulersFactory>().InSingletonScope();
      kernel.Bind<ISampleDbContext>().To<SampleDbContext>().InTransientScope().WithConstructorArgument("nameOrConnectionString", connectionString);
      
      kernel.Bind<ISqlTableDependencyProvider<Product>>().To<ProductsSqlTableDependencyProvider>()
        .InTransientScope()
        .WithConstructorArgument("connectionString", connectionString)
        .WithConstructorArgument("lifetimeScope", LifetimeScope.UniqueScope);

      kernel.Bind<ITableDependencyStatusProvider, IReactiveData<Product>, IEntityChangePublisherWithStatus<Product>, ReactiveDataWithStatus<Product>>()
        .ToConstant(ReactiveDataWithStatus<Product>.Instance);
      
      kernel.Bind<IRedisSubscriber>().To<RedisSubscriber>()
        .WithConstructorArgument("url", redisUrl);

      kernel.Bind<IDomainEntitiesSubscriber>().To<DomainEntitiesSubscriber<Product>>();
    }

    #endregion

    #region CreateShell

    protected override Window CreateShell()
    {
      return new Shell();
    }

    #endregion

    #region InitializeShell

    protected override void InitializeShell(Window shell)
    {
      base.InitializeShell(shell);
       
      if (shell is FrameworkElement shellFrameworkElement)
        shellFrameworkElement.DataContext = kernel.Get<ShellViewModel>();
    }

    #endregion

    #region ConfigureServiceLocator

    protected override void ConfigureServiceLocator()
    {
      kernel.Bind<IServiceLocator>().To<NinjectServiceLocator>().InSingletonScope();

      base.ConfigureServiceLocator();
    }

    #endregion

    #region OnExit

    protected override void OnExit(ExitEventArgs e)
    {
      base.OnExit(e);

      kernel.Dispose();
    }

    #endregion
  }
}