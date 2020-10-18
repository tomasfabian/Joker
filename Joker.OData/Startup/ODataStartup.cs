using System;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

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

    #region OnConfigureOData

    protected override void OnConfigureOData(IApplicationBuilder app)
    {
      app.UseEndpoints(endpoints =>
      {
        endpoints.EnableDependencyInjection();

        endpoints.Select().Expand().Filter().OrderBy().MaxTop(null).Count();

        if (ODataStartupSettings.EnableODataBatchHandler)
        {
          endpoints.EnableContinueOnErrorHeader();

          endpoints.MapODataRoute(ODataStartupSettings.ODataRouteName, ODataStartupSettings.ODataRoutePrefix, EdmModel,
            CreateODataBatchHandler());
        }
        else
          endpoints.MapODataRoute(ODataStartupSettings.ODataRouteName, ODataStartupSettings.ODataRoutePrefix, EdmModel);
      });
    }

    #endregion

    #endregion
  }
}