using System.Configuration;
using System.Diagnostics;
using Joker.OData.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Sample.Data.Context;
using Serilog;

namespace SelfHostedODataService
{
  public class ODataHost : ODataHost<StartupForBlazorAndOData>
  {
    protected override void OnConfigureWebHostBuilder(IWebHostBuilder webHostBuilder)
    {
      webHostBuilder
        .UseSerilog();

      base.OnConfigureWebHostBuilder(webHostBuilder);
    }

    protected override void OnHostBuilt(IHost host)
    {
      if (Debugger.IsAttached)
      {
        var connectionStringSetting = ConfigurationManager.ConnectionStrings["FargoEntities"];

        var dbContext = new SampleDbContext(connectionStringSetting.ConnectionString);

        dbContext.MigrateDatabase(connectionStringSetting.ConnectionString);

        dbContext.Dispose();
      }
    }
  }
}