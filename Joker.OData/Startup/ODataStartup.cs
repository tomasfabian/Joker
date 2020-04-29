using System;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;

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
      app.UseRouting();

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

    #region OnConfigureApp

    protected override void OnConfigureApp(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
    {
      base.OnConfigureApp(app, env, applicationLifetime);

      app.UseEndpoints(OnUseEndpoints);  
    }

    #endregion

    protected virtual void OnUseEndpoints(IEndpointRouteBuilder endpoints)
    {        
      if(StartupSettings.UseUtcTimeZone)
        endpoints.SetTimeZoneInfo(TimeZoneInfo.Utc);

      MapControllerRoutes(endpoints);

      endpoints.MapControllers();
    }

    #endregion

    #region MapControllerRoutes

    protected virtual void MapControllerRoutes(IEndpointRouteBuilder endpoints)
    {
      endpoints.MapControllerRoute(WebApiStartupSettings.WebApiRouteName, WebApiStartupSettings.WebApiRoute);
    }

    #endregion
  }
}