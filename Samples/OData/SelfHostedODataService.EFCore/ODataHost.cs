using Joker.OData.Hosting;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace SelfHostedODataService.EFCore
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