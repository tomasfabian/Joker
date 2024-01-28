using System;
using System.Data.SqlClient;
using Joker.Extensions;
using Microsoft.Extensions.Configuration;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace SelfHostedODataService.Configuration
{
  public class ProductsConfigurationProvider : IProductsConfigurationProvider
  {
    private readonly IConfiguration configuration;

    public ProductsConfigurationProvider(IConfiguration configuration)
    {
      this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    #region GetDatabaseConnectionString

    public string GetDatabaseConnectionString()
    {
      var connectionString = ConfigurationManager.ConnectionStrings["FargoEntities"].ConnectionString;

      var host = configuration["DBHOST"];

      if (host.IsNullOrEmpty())
      {
        return connectionString;
      }
      
      var sqlConnectionStringBuilder = new SqlConnectionStringBuilder
      {
        ConnectionString = connectionString,
        DataSource = host
      };

      return sqlConnectionStringBuilder.ConnectionString;
    }

    #endregion

    #region RedisUrl

    public string RedisUrl
    {
      get
      {
        var redisUrl = configuration["REDISHOST"];

        redisUrl = redisUrl ?? ConfigurationManager.AppSettings["RedisUrl"];

        return redisUrl ?? "localhost";
      }
    }

    #endregion
  }
}