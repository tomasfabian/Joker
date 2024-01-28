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
  public class ApiHost<TStartup>
    where TStartup : Startup.StartupBase
  {
    public void Run(string[] args, WebHostConfig oDataWebHostConfig)
    {
      if (oDataWebHostConfig == null) throw new ArgumentNullException(nameof(oDataWebHostConfig));

      var hostBuilder = CreateHostBuilder(args, oDataWebHostConfig);

      hostBuilder.Build()
        .OnBuilt(OnHostBuilt)
        .Run();
    }

    public async Task RunAsync(string[] args, WebHostConfig oDataWebHostConfig, CancellationToken cancellationToken = default(CancellationToken))
    {
      if (oDataWebHostConfig == null) throw new ArgumentNullException(nameof(oDataWebHostConfig));

      var hostBuilder = CreateHostBuilder(args, oDataWebHostConfig);

      await hostBuilder.Build()
        .OnBuilt(OnHostBuilt)
        .RunAsync(cancellationToken);
    }

    private IHostBuilder CreateHostBuilder(string[] args, WebHostConfig oDataWebHostConfig)
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

      OnCreateHostBuilder(hostBuilder);

      return hostBuilder;
    }
        
    protected virtual void OnCreateHostBuilder(IHostBuilder hostBuilder)
    {
    }

    protected virtual void OnConfigureWebHostBuilder(IWebHostBuilder webHostBuilder)
    {
    }

    protected virtual void OnHostBuilt(IHost host)
    {
    }
  }

  internal static class HostExtensions
  {
    internal static IHost OnBuilt(this IHost host, Action<IHost> action)
    {
      action(host);

      return host;
    }
  }
}