using System;
using System.Configuration;
using Joker.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SelfHostedODataService.Configuration
{
  public class ConfigurationProvider : IConfigurationProvider
  {
    private readonly IConfiguration configuration;

    public ConfigurationProvider(IConfiguration configuration)
    {
      this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    #region GetDatabaseConnectionString

    public string GetDatabaseConnectionString()
    {
      var connectionString = configuration.GetConnectionString("DefaultConnection");

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