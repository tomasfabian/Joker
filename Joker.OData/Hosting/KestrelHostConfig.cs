using System;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Joker.OData.Hosting
{
  public class KestrelHostConfig : WebHostConfig
  {
    public Action<KestrelServerOptions> ConfigureKestrelServer { get; set; }
  }
}