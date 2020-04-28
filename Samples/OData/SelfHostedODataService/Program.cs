using Joker.OData.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SelfHostedODataService.HostedServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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
          services.AddHostedService<SqlTableDependencyProviderHostedService>();
        }
      };

      await new ODataHost<StartupBaseWithOData>().RunAsync(args, startupSettings);
    }

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
  }
}