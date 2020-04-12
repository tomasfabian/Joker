using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace SelfHostedODataService
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var hostBuilder = Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webHostBuilder =>
        {
          webHostBuilder.ConfigureKestrel(options =>
          {
            options.AllowSynchronousIO = true;
          });

          webHostBuilder
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseIISIntegration()
            .UseStartup<StartupBaseWithOData>();
        });

      hostBuilder.Build().RunAsync();

      Console.ReadKey();
    }
  }
}