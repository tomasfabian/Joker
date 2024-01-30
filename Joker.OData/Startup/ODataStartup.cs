using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection;

namespace Joker.OData.Startup
{
  public class ODataStartup : ODataStartupBase
  {
    #region Constructors

    public ODataStartup(IWebHostEnvironment webHostEnvironment)
      : base(webHostEnvironment)
    {
    }

    #endregion

    #region Properties

    internal override bool EnableEndpointRouting { get; } = true;
    
    #endregion

    #region Methods

    #region SetSettings

    public ODataStartup SetSettings(Action<StartupSettings> setStartupSettings)
    {
      if (setStartupSettings == null) throw new ArgumentNullException(nameof(setStartupSettings));

      setStartupSettings(StartupSettings);

      return this;
    }

    public ODataStartup SetODataSettings(Action<ODataStartupSettings> setODataStartupSettings)
    {
      if (setODataStartupSettings == null) throw new ArgumentNullException(nameof(setODataStartupSettings));

      setODataStartupSettings(ODataStartupSettings);

      return this;
    }

    public ODataStartup SetWebApiSettings(Action<WebApiStartupSettings> setWebApiStartupSettings)
    {
      if (setWebApiStartupSettings == null) throw new ArgumentNullException(nameof(setWebApiStartupSettings));

      setWebApiStartupSettings(WebApiStartupSettings);

      return this;
    }
    
    #endregion

    #region OnConfigureServices

    protected override void OnConfigureServices(IServiceCollection services)
    {
      base.OnConfigureServices(services);

      OnRegisterEdmModel(services);
    }

    #endregion

    #region OnRegisterEdmModel

    protected virtual void OnRegisterEdmModel(IServiceCollection services)
    {
      services.AddSingleton(EdmModel);
    }

    #endregion

    #region OnRegisterMiddleWares

    protected override void OnRegisterMiddleWares(IApplicationBuilder app)
    {
      if (ODataStartupSettings.EnableODataBatchHandler)
        app.UseODataBatching();

      base.OnRegisterMiddleWares(app);
    }

    #endregion

    #region OnConfigureOData

    protected override void OnConfigureOData(IApplicationBuilder app)
    {
      // app.UseEndpoints(endpoints =>
      // {
      // endpoints.EnableDependencyInjection();
      //
      // endpoints.Select().Expand().Filter().OrderBy().MaxTop(null).Count();

      //    var edmModel = app.ApplicationServices.GetService<IEdmModel>() ?? EdmModel;

      //    if (ODataStartupSettings.EnableODataBatchHandler)
      //    {
      //      endpoints.EnableContinueOnErrorHeader();

      //      endpoints.MapODataRoute(ODataStartupSettings.ODataRouteName, ODataStartupSettings.ODataRoutePrefix, edmModel,
      //        CreateODataBatchHandler());
      //    }
      //    else
      //      endpoints.MapODataRoute(ODataStartupSettings.ODataRouteName, ODataStartupSettings.ODataRoutePrefix, edmModel);
      // });
    }

    #endregion

    #endregion
  }
}