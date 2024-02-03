namespace Joker.OData.Startup
{
  public class ODataStartupSettings
  {
    public bool EnableODataBatchHandler { get; private set; } = true;

    public string ODataRoutePrefix { get; private set; } = "odata";

    public ODataStartupSettings DisableODataBatchHandler()
    {
      EnableODataBatchHandler = false;

      return this;
    }

    public ODataStartupSettings SetODataRoutePrefix(string value)
    {
      ODataRoutePrefix = value;

      return this;
    }
  }
}