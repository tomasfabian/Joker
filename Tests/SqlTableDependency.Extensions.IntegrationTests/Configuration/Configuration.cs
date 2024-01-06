using Microsoft.Extensions.Configuration;
using System;

namespace SqlTableDependency.Extensions.IntegrationTests.Configuration;

public static class Configuration
{
  public static IConfigurationRoot Build()
  {
    var configurationBuilder = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

    return configurationBuilder.Build();
  }
}