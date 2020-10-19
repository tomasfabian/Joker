using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Joker.OData.Hosting
{
  public abstract class WebHostConfig
  {
    public IConfiguration Configuration { get; set; }

    public string ContentRoot { get; set; }

    public Action<IServiceCollection> ConfigureServices { get; set; }

    public string[] Urls { get; set; }
  }
}