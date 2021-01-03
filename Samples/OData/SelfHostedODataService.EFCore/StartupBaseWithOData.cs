using System;
using Autofac;
using AutoMapper;
using Joker.Factories.Schedulers;
using Joker.OData.Batch;
using Joker.OData.Extensions.OData;
using Joker.OData.Startup;
using Microsoft.AspNet.OData.Batch;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Config;
using NLog.Targets;
using Sample.DataCore.EFCore;
using Sample.Domain.Models;
using SelfHostedODataService.EFCore.AutofacModules;
using SelfHostedODataService.EFCore.Configuration;

namespace SelfHostedODataService.EFCore
{
  public class StartupBaseWithOData : ODataStartup
  {
    public StartupBaseWithOData(IWebHostEnvironment env)
      : base(env)
    { 
      SetSettings(startupSettings =>
      {
        startupSettings.UseCors = true;
      });
    }

    protected override ODataModelBuilder OnCreateEdmModel(ODataModelBuilder oDataModelBuilder)
    {
      oDataModelBuilder.Namespace = "Example";

      oDataModelBuilder.AddPluralizedEntitySet<Product>();

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
    
    protected override void OnConfigureCorsPolicy(CorsOptions options)
    {
      AddDefaultCorsPolicy(options);
    }

    protected override void OnConfigureServices(IServiceCollection services)
    {
      base.OnConfigureServices(services);
      
      var connectionString = Configuration.GetConnectionString("FargoEntities");

      services.AddDbContext<SampleDbContextCore>(options => options.UseSqlServer(connectionString).EnableSensitiveDataLogging());

      services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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