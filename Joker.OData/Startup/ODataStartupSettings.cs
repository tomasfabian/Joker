using Joker.Extensions;

namespace Joker.OData.Startup
{
  public class WebApiStartupSettings
  {    
    public string WebApiRouteName { get; private set; } = "WebApiRoute";
    public string WebApiRoutePrefix { get; private set; } = "api";
    public string WebApiTemplate { get; private set; } = "{controller}/{action}/{id?}";

    public string WebApiRoute
    {
      get
      {
        if(WebApiRoutePrefix.IsNullOrEmpty() && WebApiTemplate.IsNullOrEmpty())
          return string.Empty;

        if(WebApiRoutePrefix.IsNullOrEmpty())
          return WebApiTemplate;

        if(WebApiTemplate.IsNullOrEmpty())
          return $"{WebApiRoutePrefix}/";
        
        return $"{WebApiRoutePrefix}/{WebApiTemplate}";
      }
    }

    public WebApiStartupSettings SetWebApiRouteName(string value)
    {
      WebApiRouteName = value;

      return this;
    }

    public WebApiStartupSettings SetWebApiRoutePrefix(string webApiRoutePrefix)
    {
      WebApiRoutePrefix = webApiRoutePrefix;

      return this;
    }

    public WebApiStartupSettings SetWebApiTemplate(string webApiTemplate)
    {
      WebApiTemplate = webApiTemplate;

      return this;
    }
  }

  public class StartupSettings
  {
    public bool UseHttpsRedirection { get; private set; } = true;

    public bool UseUtcTimeZone { get; private set; } = true;

    public bool UseDeveloperExceptionPage { get; set; } = true;

    public bool UseAuthorization { get; set; } = true;

    public bool IISAllowSynchronousIO { get; set; } = true;
    
    public StartupSettings DisableHttpsRedirection()
    {
      UseHttpsRedirection = false;

      return this;
    }

    public StartupSettings DoNotUseUtcTimeZone()
    {
      UseUtcTimeZone = false;

      return this;
    }
  }

  public class ODataStartupSettings
  {
    public bool EnableODataBatchHandler { get; private set; } = true;

    public string ODataRouteName { get; private set; } = "odata";

    public string ODataRoutePrefix { get; private set; }

    public ODataStartupSettings DisableODataBatchHandler()
    {
      EnableODataBatchHandler = false;

      return this;
    }

    public ODataStartupSettings SetODataRouteName(string value)
    {
      ODataRouteName = value;

      return this;
    }

    public ODataStartupSettings SetODataRoutePrefix(string value)
    {
      ODataRoutePrefix = value;

      return this;
    }
  }
}