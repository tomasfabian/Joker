using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Joker.Disposables;
using Joker.OData.Batch;
using Joker.OData.Middleware.Logging;
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

namespace Joker.OData.Startup
{
  public abstract class ODataStartupBase : DisposableObject
  {
    #region Fields
    
    internal readonly StartupSettings StartupSettings = new StartupSettings();
    internal readonly ODataStartupSettings ODataStartupSettings = new ODataStartupSettings();
    internal readonly WebApiStartupSettings WebApiStartupSettings = new WebApiStartupSettings();

    protected IConfigurationRoot Configuration { get; }
    
    #endregion

    #region Constructors

    protected ODataStartupBase(IWebHostEnvironment env)
    {
      Configuration = new ConfigurationBuilder()
        .SetBasePath(env.ContentRootPath)
        .AddEnvironmentVariables()
        .Build();
    }

    #endregion

    #region Properties
    
    private IEdmModel edmModel;

    public IEdmModel EdmModel => edmModel ?? (edmModel = CreateEdmModel());

    internal abstract bool EnableEndpointRouting { get; }

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
    }

    #endregion

    #region ConfigureOData

    private void ConfigureOData(IApplicationBuilder app)
    {
      OnConfigureOData(app);
    }

    #region OnConfigureOData

    protected abstract void OnConfigureOData(IApplicationBuilder app);

    #endregion

    #endregion

    #region CreateODataBatchHandler

    internal ODataBatchHandler CreateODataBatchHandler()
    {
      ODataBatchHandler odataBatchHandler = OnCreateODataBatchHandler();

      return odataBatchHandler;
    }

    #endregion

    #region OnCreateODataBatchHandler

    protected virtual ODataBatchHandler OnCreateODataBatchHandler()
    {
      ODataBatchHandler odataBatchHandler = new TransactionScopeODataBatchHandler();
      
      odataBatchHandler.MessageQuotas.MaxOperationsPerChangeset = 60;
      odataBatchHandler.MessageQuotas.MaxPartsPerBatch = 10;

      return odataBatchHandler;
    }

    #endregion

    #region CreateEdmModel

    private IEdmModel CreateEdmModel()
    {
      ODataModelBuilder oDataModelBuilder = new ODataConventionModelBuilder();

      OnCreateEdmModel(oDataModelBuilder);

      return oDataModelBuilder.GetEdmModel();
    }

    #endregion

    #region OnCreateEdmModel

    protected virtual ODataModelBuilder OnCreateEdmModel(ODataModelBuilder oDataModelBuilder)
    {
      return oDataModelBuilder;
    }

    #endregion

    #region ConfigureServices

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddOptions();

      services.AddHttpContextAccessor();

      services.Configure<IISServerOptions>(options =>
      {
        options.AllowSynchronousIO = StartupSettings.IISAllowSynchronousIO; //OData doesnt support async IO in version 7.2.2        
      });

      ConfigureMvc(services);

      services.AddOData();

      OnConfigureServices(services);
    }

    #endregion

    #region OnConfigureServices

    protected virtual void OnConfigureServices(IServiceCollection services)
    {
    }

    #endregion

    #region ConfigureMvc

    private void ConfigureMvc(IServiceCollection services)
    {
      services.AddMvc(options =>
        {
          options.EnableEndpointRouting = EnableEndpointRouting;
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

    private void RegisterMiddleWares(IApplicationBuilder app)
    {
      if(ODataStartupSettings.EnableODataBatchHandler)
        app.UseODataBatching();

      OnRegisterMiddleWares(app);
    }

    protected virtual void OnRegisterMiddleWares(IApplicationBuilder app)
    {
      app.UseMiddleware<ErrorLoggerMiddleware>();
    }

    #endregion

    #region Configure

    public ILifetimeScope AutofacContainer { get; private set; }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
    {
      AutofacContainer = app.ApplicationServices.GetAutofacRoot();
 
      if (env.IsDevelopment() && StartupSettings.UseDeveloperExceptionPage)
        app.UseDeveloperExceptionPage();

      if(StartupSettings.UseAuthorization)
        app.UseAuthorization();

      RegisterMiddleWares(app);

      ConfigureOData(app);

      OnConfigureApp(app, env, applicationLifetime);
    }

    #endregion

    #region OnConfigureApp

    protected virtual void OnConfigureApp(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
    {
      if(StartupSettings.UseHttpsRedirection)
        app.UseHttpsRedirection();

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