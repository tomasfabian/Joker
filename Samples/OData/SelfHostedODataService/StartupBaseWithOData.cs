using System.Configuration;
using Microsoft.AspNet.OData.Batch;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;
using Newtonsoft.Json.Serialization;
using Sample.Data.Context;
using Sample.Domain.Models;

namespace SelfHostedODataService
{
  public class StartupBaseWithOData
  {
    #region Methods
    
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
      
      string connectionString = ConfigurationManager.ConnectionStrings["FargoEntities"].ConnectionString;

      services.AddTransient<ISampleDbContext>(_ => new SampleDbContext(connectionString));
    }

    #endregion

    #region OnConfigureServices

    protected virtual void OnConfigureServices(IServiceCollection services)
    {
      services.AddOptions();

      services.AddHttpContextAccessor();

      services.Configure<IISServerOptions>(options =>
      {
        options.AllowSynchronousIO = true; //OData doesnt support async IO in version 7.2.2        
      });

      services.Configure<FormOptions>(x =>
      {
        x.MultipartBodyLengthLimit = 4294967296;
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

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseAuthorization();

      RegisterMiddleWares(app);

      ConfigureOData(app);
    }

    #endregion

    #endregion
  }
}