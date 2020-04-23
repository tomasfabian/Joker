using System;
using System.Configuration;
using System.Data.SqlClient;
using Joker.Extensions;

namespace SelfHostedODataService.Configuration
{
  public class ProductsConfigurationProvider : IProductsConfigurationProvider
  {
    #region GetDatabaseConnectionString

    public string GetDatabaseConnectionString()
    {
      var connectionString = ConfigurationManager.ConnectionStrings["FargoEntities"].ConnectionString;

      var configuration = Environment.GetEnvironmentVariables();
      var host = configuration["DBHOST"] as string;

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
        var configuration = Environment.GetEnvironmentVariables();

        var redisUrl = configuration["REDISHOST"] as string;

        redisUrl = redisUrl ?? ConfigurationManager.AppSettings["RedisUrl"];

        return redisUrl ?? "localhost";
      }
    }

    #endregion
  }
}