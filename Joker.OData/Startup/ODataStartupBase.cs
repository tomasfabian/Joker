using System;
using Joker.OData.Batch;
using Microsoft.AspNet.OData.Batch;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;

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
      base.OnConfigureServices(services);

      services.AddOData();
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