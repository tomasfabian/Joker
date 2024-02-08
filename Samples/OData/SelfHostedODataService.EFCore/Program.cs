﻿using System;
using Joker.OData.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SelfHostedODataService.EFCore;
using SelfHostedODataService.EFCore.HostedServices;
using SelfHostedODataService.HostedServices;
using Serilog;

var startupSettings = new KestrelODataWebHostConfig()
{
  ConfigureServices = services =>
  {
    services.AddHostedService<ProductChangesHostedService>();
    services.AddHostedService<SqlTableDependencyProviderHostedService>();
  }
};

ConfigureLogging();

await new ODataHost().RunAsync(args, startupSettings);

static void ConfigureLogging()
{
  var baseDir = AppDomain.CurrentDomain.BaseDirectory;

  Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File($@"{baseDir}\logs\{nameof(ODataHost)}_.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

  Log.Information("Hello, world!");
}

#region IISODataWebHostConfig example

static ODataWebHostConfig ODataStartupConfigExample()
{
  var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

  var startupSettings = new IISODataWebHostConfig()
  {
    ConfigureServices = services => { services.AddHostedService<SqlTableDependencyProviderHostedService>(); },
    Urls = ["https://localhost:32778/"],
    Configuration = configuration
  };

  return startupSettings;
}

#endregion
