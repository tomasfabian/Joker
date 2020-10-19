using System;
using System.Data.SqlClient;
using Autofac;
using AutoMapper;
using Joker.Factories.Schedulers;
using Joker.OData.Batch;
using Joker.OData.Extensions.OData;
using Joker.OData.Startup;
using Microsoft.AspNet.OData.Batch;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NLog;
using NLog.Config;
using NLog.Targets;
using Sample.Data.Profiles;
using Sample.Data.Repositories;
using Sample.Domain.Models;
using SelfHostedODataService.AutofacModules;
using SelfHostedODataService.Configuration;

namespace SelfHostedODataService
{
  public class StartupBaseWithOData : ODataStartup
  {
    public StartupBaseWithOData(IWebHostEnvironment env)
      : base(env)
    {
    }

    protected override ODataModelBuilder OnCreateEdmModel(ODataModelBuilder oDataModelBuilder)
    {
      oDataModelBuilder.Namespace = "Example";

      oDataModelBuilder.EntitySet<Product>("Products");
      oDataModelBuilder.AddPluralizedEntitySet<Book>();
      oDataModelBuilder.AddPluralizedEntitySet<Author>();
      oDataModelBuilder.AddPluralizedEntitySet<Publisher>();
      
      oDataModelBuilder.EntityType<Publisher>().HasKey(c => new {c.PublisherId1, c.PublisherId2});

      return oDataModelBuilder;
    }
    
    private void ConfigureNLog()
    {
      var config = new LoggingConfiguration();

      // Targets where to log to: File and Console
      var logfile = new FileTarget("logfile") { FileName = "logs.txt" };
      var logconsole = new ConsoleTarget("logconsole");

      // Rules for mapping loggers to targets            
      config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
      config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

      LogManager.Configuration = config;
    }

    protected override void OnConfigureServices(IServiceCollection services)
    {
      base.OnConfigureServices(services);
      
      services.AddAutoMapper(typeof(BookProfile).Assembly);

      ConfigureNLog();
    }

    protected override void RegisterTypes(ContainerBuilder builder)
    {
      base.RegisterTypes(builder);

      ContainerBuilder.RegisterModule(new ProductsAutofacModule());

      ContainerBuilder.RegisterType<ProductsConfigurationProvider>()
        .As<IProductsConfigurationProvider>()
        .SingleInstance();

      ContainerBuilder.RegisterType<SchedulersFactory>().As<ISchedulersFactory>()
        .SingleInstance();

      builder.RegisterType<BooksMappedRepository>()
        .AsSelf()
        .InstancePerLifetimeScope();
    }

    protected override ODataBatchHandler OnCreateODataBatchHandler()
    {
      var batchHandler = (TransactionScopeODataBatchHandler)base.OnCreateODataBatchHandler();

      return batchHandler;
    }

    #region HealthCheck

    //https://localhost:5001/healthCheck
    protected override string HealthCheckPath { get; } = "/healthCheck"; // override default /health

    protected override IEndpointConventionBuilder OnMapHealthChecks(IEndpointRouteBuilder endpoints)
    {
      // return base.OnMapHealthChecks(endpoints).RequireAuthorization(); // extend default
      var healthCheckOptions = new HealthCheckOptions
      {
        AllowCachingResponses = false,
        ResultStatusCodes =
        {
          [HealthStatus.Healthy] = StatusCodes.Status200OK,
          [HealthStatus.Degraded] = StatusCodes.Status200OK,
          [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
        }
      };

      return endpoints.MapHealthChecks(HealthCheckPath, healthCheckOptions);
    }

    protected override void ConfigureHealthChecks(IHealthChecksBuilder healthChecksBuilder)
    {
      healthChecksBuilder.AddAsyncCheck("SqlServer", async () =>
      {
        var connectionString = new ProductsConfigurationProvider(Configuration).GetDatabaseConnectionString();

        using (var connection = new SqlConnection(connectionString))
        {
          try
          {
            await connection.OpenAsync();
            
            return HealthCheckResult.Healthy();
          }
          catch (SqlException)
          {
            return HealthCheckResult.Unhealthy();
          }
        }
      });
    }

    #endregion
  }
}