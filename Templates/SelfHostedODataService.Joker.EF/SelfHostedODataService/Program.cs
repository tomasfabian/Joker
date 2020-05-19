using System;
using System.Threading.Tasks;
using Joker.OData.Hosting;
using Serilog;

namespace SelfHostedODataService
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      var startupSettings = new KestrelODataWebHostConfig()
      {
      };
      
      ConfigureLogging();

      await new ODataHost().RunAsync(args, startupSettings);
    }

    private static void ConfigureLogging()
    {
      var baseDir = AppDomain.CurrentDomain.BaseDirectory;

      Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .WriteTo.File($@"{baseDir}\logs\{nameof(ODataHost)}_.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();

      Log.Information("Hello, world!");
    }
  }
}