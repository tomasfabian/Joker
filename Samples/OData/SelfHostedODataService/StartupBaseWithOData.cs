using System.Data;
using Autofac;
using Joker.Factories.Schedulers;
using Joker.OData.Batch;
using Joker.OData.Extensions.OData;
using Joker.OData.Startup;
using Microsoft.AspNet.OData.Batch;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Config;
using NLog.Targets;
using Sample.Domain.Models;
using SelfHostedODataService.AutofacModules;
using SelfHostedODataService.Configuration;

namespace SelfHostedODataService
{
  public class StartupBaseWithOData : ODataStartup
  {
    public StartupBaseWithOData(IWebHostEnvironment env)
      : base(env)
    {
    }

    protected override ODataModelBuilder OnCreateEdmModel(ODataModelBuilder oDataModelBuilder)
    {
      oDataModelBuilder.Namespace = "Example";

      oDataModelBuilder.EntitySet<Product>("Products");
      oDataModelBuilder.AddPluralizedEntitySet<Book>();
      oDataModelBuilder.AddPluralizedEntitySet<Author>();
      oDataModelBuilder.AddPluralizedEntitySet<Publisher>();
      
      oDataModelBuilder.EntityType<Publisher>().HasKey(c => new {c.PublisherId1, c.PublisherId2});

      return oDataModelBuilder;
    }
    
    private void ConfigureNLog()
    {
      var config = new LoggingConfiguration();

      // Targets where to log to: File and Console
      var logfile = new FileTarget("logfile") { FileName = "logs.txt" };
      var logconsole = new ConsoleTarget("logconsole");

      // Rules for mapping loggers to targets            
      config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
      config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

      LogManager.Configuration = config;
    }

    protected override void OnConfigureServices(IServiceCollection services)
    {
      base.OnConfigureServices(services);

      ConfigureNLog();
    }

    protected override void RegisterTypes(ContainerBuilder builder)
    {
      base.RegisterTypes(builder);

      ContainerBuilder.RegisterModule(new ProductsAutofacModule());

      ContainerBuilder.RegisterType<ProductsConfigurationProvider>()
        .As<IProductsConfigurationProvider>()
        .SingleInstance();

      ContainerBuilder.RegisterType<SchedulersFactory>().As<ISchedulersFactory>()
        .SingleInstance();
    }

    protected override ODataBatchHandler OnCreateODataBatchHandler()
    {
      var batchHandler = (TransactionScopeODataBatchHandler)base.OnCreateODataBatchHandler();

      return batchHandler;
    }
  }
}