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
}