using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Joker.OData.Startup;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Joker.OData.Hosting
{
  public class ODataHost<TStartup>
    where TStartup : ODataStartupBase
  {
    public void Run(string[] args, ODataWebHostConfig oDataWebHostConfig)
    {
      if (oDataWebHostConfig == null) throw new ArgumentNullException(nameof(oDataWebHostConfig));

      var hostBuilder = CreateHostBuilder(args, oDataWebHostConfig);

      hostBuilder.Build()
        .Run();
    }

    public async Task RunAsync(string[] args, ODataWebHostConfig oDataWebHostConfig, CancellationToken cancellationToken = default(CancellationToken))
    {
      if (oDataWebHostConfig == null) throw new ArgumentNullException(nameof(oDataWebHostConfig));

      var hostBuilder = CreateHostBuilder(args, oDataWebHostConfig);

      await hostBuilder.Build()
        .RunAsync(cancellationToken);
    }

    private IHostBuilder CreateHostBuilder(string[] args, ODataWebHostConfig oDataWebHostConfig)
    {
      oDataWebHostConfig.ContentRoot = oDataWebHostConfig.ContentRoot ?? Directory.GetCurrentDirectory();

      var hostBuilder = Host.CreateDefaultBuilder(args)
        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
        .ConfigureWebHostDefaults(webHostBuilder =>
        {
          if (oDataWebHostConfig.Configuration != null)
            webHostBuilder.UseConfiguration(oDataWebHostConfig.Configuration);

          if (oDataWebHostConfig.Urls != null && oDataWebHostConfig.Urls.Any())
            webHostBuilder.UseUrls(oDataWebHostConfig.Urls);

          if (oDataWebHostConfig is KestrelODataWebHostConfig kestrelConfig)
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
            .UseContentRoot(oDataWebHostConfig.ContentRoot ?? Directory.GetCurrentDirectory())
            .UseStartup<TStartup>()
            .ConfigureServices(oDataWebHostConfig?.ConfigureServices ?? (s => { }));

          OnConfigureWebHostBuilder(webHostBuilder);
        });

      return hostBuilder;
    }

    protected virtual void OnConfigureWebHostBuilder(IWebHostBuilder webHostBuilder)
    {
    }
  }
}