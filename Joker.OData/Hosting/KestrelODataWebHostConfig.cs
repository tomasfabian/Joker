using System;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Joker.OData.Hosting
{
  public class KestrelODataWebHostConfig : ODataWebHostConfig
  {
    public Action<KestrelServerOptions> ConfigureKestrelServer { get; set; }
  }
}