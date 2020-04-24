using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SelfHostedODataService.HostedServices;

namespace SelfHostedODataService
{
  public class Program
  {
    private static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

    public static async Task Main(string[] args)
    {
      var hostBuilder = Host.CreateDefaultBuilder(args)
        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
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
            .UseStartup<StartupBaseWithOData>()
            .ConfigureServices(services =>
            {
              services.AddHostedService<SqlTableDependencyProviderHostedService>();
            });
        });

      await hostBuilder.Build().RunAsync();
    }
  }
}