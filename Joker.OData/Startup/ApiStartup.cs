using System;
using Microsoft.AspNetCore.Hosting;

namespace Joker.OData.Startup
{
  public abstract class ApiStartup : StartupBase
  {
    protected ApiStartup(IWebHostEnvironment env, bool enableEndpointRouting = true) 
      : base(env)
    {
      EnableEndpointRouting = enableEndpointRouting;
    }

    internal override bool EnableEndpointRouting { get; }

    #region SetSettings

    public StartupBase SetSettings(Action<StartupSettings> setStartupSettings)
    {
      if (setStartupSettings == null) throw new ArgumentNullException(nameof(setStartupSettings));

      setStartupSettings(StartupSettings);

      return this;
    }

    public StartupBase SetWebApiSettings(Action<WebApiStartupSettings> setWebApiStartupSettings)
    {
      if (setWebApiStartupSettings == null) throw new ArgumentNullException(nameof(setWebApiStartupSettings));

      setWebApiStartupSettings(WebApiStartupSettings);

      return this;
    }
    
    #endregion
  }
}