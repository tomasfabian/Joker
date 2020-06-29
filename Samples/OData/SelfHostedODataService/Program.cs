using System;
using Joker.OData.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SelfHostedODataService.HostedServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace SelfHostedODataService
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      var startupSettings = new KestrelODataWebHostConfig()
      {
        ConfigureServices = services =>
        {
          services.AddHostedService<ProductChangesHostedService>();
          services.AddHostedService<SqlTableDependencyProviderHostedService>();
        }
      };
      
      ConfigureLogging();

      await new ODataHost().RunAsync(args, startupSettings);
    }

    private static void ConfigureLogging()
    {
      var baseDir = AppDomain.CurrentDomain.BaseDirectory;

      Log.Logger = new LoggerConfiguration()
        // .MinimumLevel.Debug()
        .MinimumLevel.Warning()
        .WriteTo.Console()
        .WriteTo.File($@"{baseDir}\logs\{nameof(ODataHost)}_.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();

      Log.Information("Hello, world!");
    }

    #region IISODataWebHostConfig example

    private static ODataWebHostConfig ODataStartupConfigExample()
    {
      var configuration = new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .Build();

      var startupSettings = new IISODataWebHostConfig()
      {
        ConfigureServices = services => { services.AddHostedService<SqlTableDependencyProviderHostedService>(); },
        Urls = new[] { @"https://localhost:32778/" },
        Configuration = configuration
      };

      return startupSettings;
    }

    #endregion
  }
}