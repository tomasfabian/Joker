using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace SelfHostedODataService
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      var hostBuilder = Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webHostBuilder =>
        {
          webHostBuilder.ConfigureKestrel(options =>
          {
            options.AllowSynchronousIO = true;
          });

          webHostBuilder
            .UseKestrel()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseIISIntegration()
            .UseStartup<StartupBaseWithOData>();
        });

      await hostBuilder.Build().RunAsync();
    }
  }
}