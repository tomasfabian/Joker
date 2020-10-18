using Joker.OData.Startup;

namespace Joker.OData.Hosting
{
  public class ODataHost<TStartup> : ApiHost<TStartup>
    where TStartup : ODataStartupBase
  {
  }
}