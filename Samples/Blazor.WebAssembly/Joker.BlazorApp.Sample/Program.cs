using Autofac;
using Autofac.Extensions.DependencyInjection;
using Joker.Contracts;
using Joker.MVVM.ViewModels;
using Joker.Platforms.Factories.Schedulers;
using Joker.PubSubUI.Shared.Factories.ViewModels;
using Joker.PubSubUI.Shared.ViewModels.Products;
using Joker.Reactive;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Sample.Domain.Models;
using Joker.BlazorApp.Sample;
using Joker.BlazorApp.Sample.Factories.Schedulers;
using Joker.BlazorApp.Sample.Navigation;
using Joker.BlazorApp.Sample.Subscribers;
using Joker.Factories.Schedulers;
using Joker.Notifications;
using Joker.PubSubUI.Shared.Navigation;
using Microsoft.OData.Edm;
using OData.Client;
using ReactiveListViewModelFactory = Joker.BlazorApp.Sample.Factories.ViewModels.ReactiveListViewModelFactory;
using ViewModelsFactory = Joker.BlazorApp.Sample.Factories.ViewModels.ViewModelsFactory;
using Microsoft.AspNetCore.Components.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Logging.SetMinimumLevel(LogLevel.Error);

var odataUrl = builder.Configuration["ODataUrl"];

var httpClient = new HttpClient { BaseAddress = new Uri(odataUrl) };

var serviceModel = await ODataServiceContext.GetServiceModelAsync(httpClient);

ConfigureContainer(builder, serviceModel);

builder.Services.AddScoped(sp => httpClient);

await builder.Build().RunAsync();

static void ConfigureContainer(WebAssemblyHostBuilder builder, IEdmModel serviceModel)
{
    builder.ConfigureContainer(new AutofacServiceProviderFactory(), containerBuilder =>
    {
        containerBuilder.RegisterType<Joker.BlazorApp.Sample.Factories.OData.ODataServiceContextFactory>()
            .As<IODataServiceContextFactory>()
            .SingleInstance();

        containerBuilder.Register(c => serviceModel)
            .As<IEdmModel>()
            .SingleInstance();

        containerBuilder.RegisterType<ReactiveListViewModelFactory>()
            .As<IReactiveListViewModelFactory<ProductViewModel>>()
            .SingleInstance();

        containerBuilder.RegisterType<PlatformSchedulersFactory>()
            .As<IPlatformSchedulersFactory, ISchedulersFactory>()
            .SingleInstance();

        containerBuilder.RegisterType<Joker.BlazorApp.Sample.Navigation.DialogManager>()
            .As<IBlazorDialogManager, IDialogManager>()
            .SingleInstance();

        containerBuilder.Register(c => ReactiveDataWithStatus<Product>.Instance)
            .As<IReactiveData<Product>, IEntityChangePublisherWithStatus<Product>, ITableDependencyStatusProvider>()
            .SingleInstance();

        containerBuilder.RegisterType<ProductsEntityChangesViewModel>().AsSelf();
        containerBuilder.RegisterType<ReactiveProductsViewModel>().AsSelf();
        containerBuilder.RegisterType<ProductViewModel>().AsSelf();
        containerBuilder.RegisterType<ViewModelsFactory>().As<IViewModelsFactory>();

        containerBuilder.RegisterType<DomainEntitiesSubscriber<Product>>()
            .WithParameter(new NamedParameter("url", builder.Configuration["ODataUrl"]))
            .As<IDomainEntitiesSubscriber>();
    });
}