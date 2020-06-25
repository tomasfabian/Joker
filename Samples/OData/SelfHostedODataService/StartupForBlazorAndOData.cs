using System.Linq;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SelfHostedODataService.Configuration;
using SelfHostedODataService.SignalR.Hubs;
using StackExchange.Redis;

namespace SelfHostedODataService
{
  public class StartupForBlazorAndOData : StartupBaseWithOData
  {
    public StartupForBlazorAndOData(IWebHostEnvironment env) 
      : base(env)
    {
    }

    protected override void RegisterTypes(ContainerBuilder builder)
    {
      base.RegisterTypes(builder);

      builder
        .Register(ctx => ConnectionMultiplexer.Connect(ctx.Resolve<IProductsConfigurationProvider>().RedisUrl))
        .As<IConnectionMultiplexer>()
        .SingleInstance();
    }

    protected override void OnConfigureServices(IServiceCollection services)
    {
      base.OnConfigureServices(services);

      services.AddControllersWithViews();
      services.AddRazorPages();

      services.AddSignalR();

      services.AddResponseCompression(opts =>
      {
        opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
          new[] { "application/octet-stream" });
      });
    }

    protected override void OnConfigureApp(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
    {
      base.OnConfigureApp(app, env, applicationLifetime);

      if (env.IsDevelopment())
      {
        app.UseWebAssemblyDebugging();
      }

      app.UseResponseCompression();

      app.UseBlazorFrameworkFiles();
      app.UseStaticFiles();
    }

    protected override void OnUseEndpoints(IEndpointRouteBuilder endpoints)
    {
      base.OnUseEndpoints(endpoints);

      endpoints.MapHub<DataChangesHub>("/dataChangesHub");

      endpoints.MapRazorPages();
      endpoints.MapControllers();
      endpoints.MapFallbackToFile("index.html");
    }
  }
}