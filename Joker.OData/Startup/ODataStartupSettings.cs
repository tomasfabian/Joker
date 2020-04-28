namespace Joker.OData.Startup
{
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