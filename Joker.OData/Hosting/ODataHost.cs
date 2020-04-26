using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Joker.OData.Hosting
{
  public class ODataHost<TStartup>
    where TStartup : ODataStartup
  {
    public void Run(string[] args, ODataStartupConfig oDataStartupConfig)
    {
      if (oDataStartupConfig == null) throw new ArgumentNullException(nameof(oDataStartupConfig));

      var hostBuilder = CreateHostBuilder(args, oDataStartupConfig);

      hostBuilder.Build()
        .Run();
    }

    public async Task RunAsync(string[] args, ODataStartupConfig oDataStartupConfig, CancellationToken cancellationToken = default(CancellationToken))
    {
      if (oDataStartupConfig == null) throw new ArgumentNullException(nameof(oDataStartupConfig));

      var hostBuilder = CreateHostBuilder(args, oDataStartupConfig);

      await hostBuilder.Build()
        .RunAsync(cancellationToken);
    }

    private IHostBuilder CreateHostBuilder(string[] args, ODataStartupConfig oDataStartupConfig)
    {
      oDataStartupConfig.ContentRoot = oDataStartupConfig.ContentRoot ?? Directory.GetCurrentDirectory();

      var hostBuilder = Host.CreateDefaultBuilder(args)
        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
        .ConfigureWebHostDefaults(webHostBuilder =>
        {
          if (oDataStartupConfig.Configuration != null)
            webHostBuilder.UseConfiguration(oDataStartupConfig.Configuration);

          if (oDataStartupConfig.Urls != null && oDataStartupConfig.Urls.Any())
            webHostBuilder.UseUrls(oDataStartupConfig.Urls);

          if (oDataStartupConfig is KestrelODataStartupConfig kestrelConfig)
          {
            kestrelConfig.ConfigureKestrelServer = kestrelConfig.ConfigureKestrelServer ?? (options => { });

            webHostBuilder
              .UseKestrel(kestrelConfig.ConfigureKestrelServer);
          }
          else
          {
            webHostBuilder
              .UseIISIntegration();
          }
          
          webHostBuilder
            .UseContentRoot(oDataStartupConfig.ContentRoot ?? Directory.GetCurrentDirectory())
            .UseStartup<TStartup>()
            .ConfigureServices(oDataStartupConfig?.ConfigureServices ?? (s => { }));

          OnConfigureWebHostBuilder(webHostBuilder);
        });

      return hostBuilder;
    }

    protected virtual void OnConfigureWebHostBuilder(IWebHostBuilder webHostBuilder)
    {
    }
  }
}