using System;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Joker.OData.Hosting
{
  public class KestrelODataStartupConfig : ODataStartupConfig
  {
    public Action<KestrelServerOptions> ConfigureKestrelServer { get; set; }
  }
}