using System;
using Joker.OData.Batch;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.OData;
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

    #region OnConfigureServices

    protected override void OnConfigureServices(IServiceCollection services)
    {
      IEdmModel model = null;
      services
        .AddOptions<ODataOptions>()
        .Configure<IEdmModel>((odataOptions, edmModel) =>
        {
          model = edmModel ?? EdmModel;
        });

      base.OnConfigureServices(services);

      var oDataBuilder = services.AddControllers()
        .AddOData(options =>
        {
          if (StartupSettings.UseUtcTimeZone)
            options.TimeZone = TimeZoneInfo.Utc;

          options.EnableContinueOnErrorHeader = ODataStartupSettings.EnableODataBatchHandler;

          options.Select().Expand().Filter().OrderBy().SetMaxTop(null).Count();

          if (ODataStartupSettings.EnableODataBatchHandler)
          {
            options.AddRouteComponents(ODataStartupSettings.ODataRoutePrefix, model, CreateODataBatchHandler());
          }
          else
          {
            options.AddRouteComponents(ODataStartupSettings.ODataRoutePrefix, model);
          }
        });

      // oDataBuilder.AddConvention<RefRoutingConvention>();
      // oDataBuilder.AddConvention<KeylessEntityRoutingConvention>();
    }

    #endregion

    #region OnConfigureApp

    protected override void OnConfigureApp(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
    {
      ConfigureOData(app);

      base.OnConfigureApp(app, env, applicationLifetime);
    }

    #endregion

    #endregion
  }
}