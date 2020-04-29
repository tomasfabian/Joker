using System;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;

namespace Joker.OData.Startup
{
  public class ODataStartupLegacy : ODataStartupBase
  {
    #region Constructors

    public ODataStartupLegacy(IWebHostEnvironment webHostEnvironment)
      : base(webHostEnvironment)
    {
    }

    #endregion

    #region Properties

    internal override bool EnableEndpointRouting { get; } = false;
    
    #endregion

    #region Methods

    #region SetSettings

    public ODataStartupLegacy SetSettings(Action<StartupSettings> setStartupSettings)
    {
      if (setStartupSettings == null) throw new ArgumentNullException(nameof(setStartupSettings));

      setStartupSettings(StartupSettings);

      return this;
    }

    public ODataStartupLegacy SetODataSettings(Action<ODataStartupSettings> setODataStartupSettings)
    {
      if (setODataStartupSettings == null) throw new ArgumentNullException(nameof(setODataStartupSettings));

      setODataStartupSettings(ODataStartupSettings);

      return this;
    }

    public ODataStartupLegacy SetWebApiSettings(Action<WebApiStartupSettings> setWebApiStartupSettings)
    {
      if (setWebApiStartupSettings == null) throw new ArgumentNullException(nameof(setWebApiStartupSettings));

      setWebApiStartupSettings(WebApiStartupSettings);

      return this;
    }
    
    #endregion

    #region OnConfigureOData

    protected override void OnConfigureOData(IApplicationBuilder app)
    {        
      app.UseMvc(routeBuilder =>
      {
        routeBuilder.EnableDependencyInjection();

        routeBuilder.Select().Expand().Filter().OrderBy().MaxTop(null).Count();

        if (ODataStartupSettings.EnableODataBatchHandler)
        {
          routeBuilder.EnableContinueOnErrorHeader();

          routeBuilder.MapODataServiceRoute(ODataStartupSettings.ODataRouteName, ODataStartupSettings.ODataRoutePrefix, EdmModel, CreateODataBatchHandler());
        }
        else
          routeBuilder.MapODataServiceRoute(ODataStartupSettings.ODataRouteName, ODataStartupSettings.ODataRoutePrefix, EdmModel);
      });
    }

    #endregion

    #region OnConfigureApp

    protected override void OnConfigureApp(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
    {
      app.UseMvc(routeBuilder =>
      {
        MapRoutes(routeBuilder);

        if(StartupSettings.UseUtcTimeZone)
          routeBuilder.SetTimeZoneInfo(TimeZoneInfo.Utc);
      });  

      base.OnConfigureApp(app, env, applicationLifetime);
    }

    #endregion

    #region MapRoutes

    protected virtual void MapRoutes(IRouteBuilder routeBuilder)
    {
      routeBuilder.MapRoute(WebApiStartupSettings.WebApiRouteName, WebApiStartupSettings.WebApiRoute);
    }

    #endregion

    #endregion
  }
}