using Joker.OData.Hosting;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace SelfHostedODataService.Joker.EF
{
  public class ODataHost : ODataHost<StartupBaseWithOData>
  {
    protected override void OnConfigureWebHostBuilder(IWebHostBuilder webHostBuilder)
    {
      webHostBuilder
        .UseSerilog();

      base.OnConfigureWebHostBuilder(webHostBuilder);
    }
  }
}