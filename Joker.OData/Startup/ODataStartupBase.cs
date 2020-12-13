using System;
using Autofac;
using Joker.OData.Batch;
using Joker.OData.Routing.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.OData.Routing.Conventions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Joker.OData.Startup
{
  public abstract class ODataStartupBase : StartupBase
  {
    #region Fields
    
    internal readonly ODataStartupSettings ODataStartupSettings = new ODataStartupSettings();
    
    #endregion

    #region Constructors

    protected ODataStartupBase(IWebHostEnvironment env)
      : base(env)
    {
    }

    #endregion

    #region Properties
    
    private IEdmModel edmModel;

    public IEdmModel EdmModel => edmModel ?? (edmModel = CreateEdmModel());

    #endregion

    #region Methods

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

    #region OnConfigureServices

    protected override void OnConfigureServices(IServiceCollection services)
    {
      OnRegisterEdmModel(services);

      IEdmModel model = null;
      services
        .AddOptions<ODataOptions>()
        .Configure<IEdmModel>((odataOptions, edmModel) =>
        {
          model = edmModel ?? EdmModel;
        });

      base.OnConfigureServices(services);

      var oDataBuilder = services.AddOData(options =>
      {
        if(StartupSettings.UseUtcTimeZone)
          options.SetTimeZoneInfo(TimeZoneInfo.Utc);

        options.EnableContinueOnErrorHeader = ODataStartupSettings.EnableODataBatchHandler;

        options.Select().Expand().Filter().OrderBy().SetMaxTop(null).Count();

        if (ODataStartupSettings.EnableODataBatchHandler)
        {
          options.AddModel(ODataStartupSettings.ODataRoutePrefix, model,
            CreateODataBatchHandler());
        }
        else
        {
          options.AddModel(ODataStartupSettings.ODataRoutePrefix, model,
            CreateODataBatchHandler());
        }
      });

      oDataBuilder.AddConvention<RefRoutingConvention>();
      oDataBuilder.AddConvention<KeylessEntityRoutingConvention>();
    }

    #endregion

    #region OnRegisterEdmModel

    //protected virtual void OnRegisterEdmModel(IServiceCollection services)
    //{
    //  services.AddSingleton(EdmModel);
    //}

    #endregion

    #endregion
  }
}