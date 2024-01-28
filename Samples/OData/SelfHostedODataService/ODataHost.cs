using System.Diagnostics;
using Joker.OData.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Sample.Data.Context;
using SelfHostedODataService.Configuration;
using Serilog;

namespace SelfHostedODataService
{
  public class ODataHost : ODataHost<ODataStartupWithPubSub>
  {
    protected override void OnCreateHostBuilder(IHostBuilder hostBuilder)
    {
        hostBuilder
            .UseSerilog();
      
      base.OnCreateHostBuilder(hostBuilder);
    }

    protected override void OnHostBuilt(IHost host)
    {
      if (Debugger.IsAttached)
      {
        var configuration = new ConfigurationBuilder()
          .AddEnvironmentVariables()
          .Build();
        
        var connectionString = new ProductsConfigurationProvider(configuration).GetDatabaseConnectionString();

        var dbContext = new SampleDbContext(connectionString);

        dbContext.MigrateDatabase(connectionString);

        dbContext.Dispose();
      }
    }
  }
}