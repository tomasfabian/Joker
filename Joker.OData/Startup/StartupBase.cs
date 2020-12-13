using System;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Joker.Disposables;
using Joker.OData.Middleware.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;

namespace Joker.OData.Startup
{
  public abstract class StartupBase : DisposableObject
  {
    #region Fields
    
    internal readonly StartupSettings StartupSettings = new StartupSettings();
    internal readonly WebApiStartupSettings WebApiStartupSettings = new WebApiStartupSettings();

    protected IConfigurationRoot Configuration { get; }
    
    #endregion

    #region Constructors

    protected StartupBase(IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
        StartupSettings.DisableHttpsRedirection();

      Configuration = new ConfigurationBuilder()
        .SetBasePath(env.ContentRootPath)
        .AddEnvironmentVariables()
        .AddJsonFile("appsettings.json", optional:true, reloadOnChange:true)
        .Build();
    }

    #endregion

    #region Properties

    internal abstract bool EnableEndpointRouting { get; }

    public ILifetimeScope AutofacContainer { get; private set; }

    protected virtual string HealthCheckPath { get; } = "/health";

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

      if (StartupSettings.UseHealthChecks)
        ConfigureHealthChecks(services.AddHealthChecks());

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

    #region ConfigureHealthChecks

    protected virtual void ConfigureHealthChecks(IHealthChecksBuilder healthChecksBuilder)
    {
    }

    #endregion
    
    #region RegisterMiddleWares

    private void RegisterMiddleWares(IApplicationBuilder app)
    {
      OnRegisterMiddleWares(app);
    }

    protected virtual void OnRegisterMiddleWares(IApplicationBuilder app)
    {
      app.UseMiddleware<ErrorLoggerMiddleware>();
    }
    
    #endregion

    #region Configure

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
    {
      AutofacContainer = app.ApplicationServices.GetAutofacRoot();
 
      if (env.IsDevelopment() && StartupSettings.UseDeveloperExceptionPage)
        app.UseDeveloperExceptionPage();

      RegisterMiddleWares(app);

      OnAddExtensions(app);

      OnConfigureApp(app, env, applicationLifetime);
    }

    #endregion

    #region OnAddExtensions

    protected virtual void OnAddExtensions(IApplicationBuilder app)
    {
      if(EnableEndpointRouting)
        app.UseRouting();
      
      if(StartupSettings.UseAuthentication)
        app.UseAuthentication();

      if(StartupSettings.UseAuthorization)
        app.UseAuthorization();
    }

    #endregion

    #region OnUseEndpoints

    protected virtual void OnUseEndpoints(IEndpointRouteBuilder endpoints)
    {
      if (!EnableEndpointRouting)
        return;

      MapControllerRoutes(endpoints);

      endpoints.MapControllers();

      if (StartupSettings.UseHealthChecks)
        OnMapHealthChecks(endpoints);
    }

    #endregion

    #region OnMapHealthChecks

    protected virtual IEndpointConventionBuilder OnMapHealthChecks(IEndpointRouteBuilder endpoints)
    {
      return endpoints.MapHealthChecks(HealthCheckPath);
    }

    #endregion

    #region MapControllerRoutes

    protected virtual void MapControllerRoutes(IEndpointRouteBuilder endpoints)
    {
      if (!EnableEndpointRouting)
        return;

      endpoints.MapControllerRoute(WebApiStartupSettings.WebApiRouteName, WebApiStartupSettings.WebApiRoute);
    }

    #endregion

    #region OnConfigureApp

    protected virtual void OnConfigureApp(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
    {
      if(StartupSettings.UseHttpsRedirection)
        app.UseHttpsRedirection();

      applicationLifetime.ApplicationStopping.Register(OnShutdown);

      if(EnableEndpointRouting)
        app.UseEndpoints(OnUseEndpoints);

      if (StartupSettings.UseHealthChecks && !EnableEndpointRouting)
        OnUseHealthChecks(app);
    }

    protected virtual void OnUseHealthChecks(IApplicationBuilder app)
    {
      app.UseHealthChecks(HealthCheckPath);
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