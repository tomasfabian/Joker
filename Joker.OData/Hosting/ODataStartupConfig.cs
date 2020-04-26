using System;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Joker.OData.Hosting
{
  public class ODataStartupConfig
  {
    public IConfiguration Configuration { get; set; }

    public string ContentRoot { get; set; }

    public Action<IServiceCollection> ConfigureServices { get; set; }

    public Action<KestrelServerOptions> ConfigureKestrelServer { get; set; }

    public string[] Urls { get; set; }
  }
}