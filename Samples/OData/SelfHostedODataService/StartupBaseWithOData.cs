using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Joker.Disposables;
using Joker.Factories.Schedulers;
using Microsoft.AspNet.OData.Batch;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;
using Newtonsoft.Json.Serialization;
using NLog;
using NLog.Config;
using NLog.Targets;
using Sample.Domain.Models;
using SelfHostedODataService.AutofacModules;
using SelfHostedODataService.Configuration;

namespace SelfHostedODataService
{
  public class StartupBaseWithOData : DisposableObject
  {
    #region Fields

    private readonly IConfigurationRoot configuration;

    #endregion

    #region Constructors

    public StartupBaseWithOData(IWebHostEnvironment env)
    {
      configuration = new ConfigurationBuilder()
        .SetBasePath(env.ContentRootPath)
        .AddEnvironmentVariables()
        .Build();
    }

    #endregion

    #region Methods    

    #region ConfigureContainer

    protected ContainerBuilder ContainerBuilder;

    public void ConfigureContainer(ContainerBuilder builder)
    {
      ContainerBuilder = builder;

      RegisterTypes(builder);
    }

    #endregion

    #region RegisterTypes

    protected virtual void RegisterTypes(ContainerBuilder builder)
    {
      ContainerBuilder.RegisterModule(new ProductsAutofacModule());

      ContainerBuilder.RegisterType<ProductsConfigurationProvider>()
        .As<IProductsConfigurationProvider>()
        .SingleInstance();

      ContainerBuilder.RegisterInstance<IConfiguration>(configuration);

      ContainerBuilder.RegisterType<SchedulersFactory>().As<ISchedulersFactory>()
        .SingleInstance();
    }

    #endregion

    #region ConfigureOData

    private IEdmModel edmModel;

    private void ConfigureOData(IApplicationBuilder app)
    {
      edmModel = CreateEdmModel();

      app.UseMvc(routeBuilder =>
      {
        routeBuilder.EnableDependencyInjection();

        routeBuilder.Select().Expand().Filter().OrderBy().MaxTop(null).Count();

        routeBuilder.EnableContinueOnErrorHeader();

        routeBuilder.MapODataServiceRoute("odata", null, edmModel, CreateODataBatchHandler());
      });
    }

    #endregion

    #region CreateODataBatchHandler

    private ODataBatchHandler CreateODataBatchHandler()
    {
      ODataBatchHandler odataBatchHandler = new DefaultODataBatchHandler();

      odataBatchHandler.MessageQuotas.MaxOperationsPerChangeset = 60;
      odataBatchHandler.MessageQuotas.MaxPartsPerBatch = 10;

      return odataBatchHandler;
    }

    #endregion

    #region CreateEdmModel

    private IEdmModel CreateEdmModel()
    {
      ODataModelBuilder oDataModelBuilder = new ODataConventionModelBuilder();

      oDataModelBuilder.Namespace = "Example";

      oDataModelBuilder.EntitySet<Product>("Products");

      return oDataModelBuilder.GetEdmModel();
    }

    #endregion

    #region ConfigureServices

    public void ConfigureServices(IServiceCollection services)
    {
      OnConfigureServices(services);
    }

    #endregion

    #region ConfigureNLog

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

    #endregion

    #region OnConfigureServices

    protected virtual void OnConfigureServices(IServiceCollection services)
    {
      ConfigureNLog();

      services.AddOptions();

      services.AddHttpContextAccessor();

      services.Configure<IISServerOptions>(options =>
      {
        options.AllowSynchronousIO = true; //OData doesnt support async IO in version 7.2.2        
      });

      ConfigureMvc(services);

      services.AddOData();
    }

    #endregion

    #region ConfigureMvc

    private void ConfigureMvc(IServiceCollection services)
    {
      services.AddMvc(options =>
        {
          options.EnableEndpointRouting = false;
        })
        .AddControllersAsServices()
        .AddNewtonsoftJson(ConfigureNewtonsoftJson)
        .SetCompatibilityVersion(CompatibilityVersion.Latest);
    }

    #endregion

    #region ConfigureNewtonsoftJson

    protected virtual void ConfigureNewtonsoftJson(MvcNewtonsoftJsonOptions options)
    {
      options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    }

    #endregion

    #region RegisterMiddleWares

    protected void RegisterMiddleWares(IApplicationBuilder app)
    {
      app.UseODataBatching();
    }

    #endregion

    #region Configure

    public ILifetimeScope AutofacContainer { get; private set; }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
    {
      AutofacContainer = app.ApplicationServices.GetAutofacRoot();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseAuthorization();

      RegisterMiddleWares(app);

      ConfigureOData(app);

      OnConfigureApp(app, env, applicationLifetime);
    }

    #endregion

    #region OnConfigureApp

    protected virtual void OnConfigureApp(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
    {
      app.UseMvc(routeBuilder =>
      {
        routeBuilder.MapRoute("WebApiRoute", "api/{controller}/{action}/{id?}");

        routeBuilder.SetTimeZoneInfo(TimeZoneInfo.Utc);
      });

      applicationLifetime.ApplicationStopping.Register(OnShutdown);
    }

    #endregion

    #region OnShutdown

    private void OnShutdown()
    {
      Dispose();
    }

    #endregion

    #region OnDispose

    protected override void OnDispose()
    {
      base.OnDispose();

      AutofacContainer.Dispose();
    }

    #endregion

    #endregion
  }
}