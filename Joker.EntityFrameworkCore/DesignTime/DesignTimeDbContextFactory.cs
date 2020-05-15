using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Joker.EntityFrameworkCore.DesignTime
{
  public abstract class DesignTimeDbContextFactory<TContext> : IDesignTimeDbContextFactory<TContext> 
    where TContext : DbContext
  {
    public TContext CreateDbContext(string[] args)
    {
      string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

      IConfigurationRoot configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{environment}.json", optional: true)
        .Build();
 
      var builder = new DbContextOptionsBuilder<TContext>();
 
      var connectionString = configuration.GetConnectionString(ConnectionStringName);
 
      builder.UseSqlServer(connectionString);

      return Create(builder.Options);
    }

    protected abstract TContext Create(DbContextOptions<TContext> options);

    public string ConnectionStringName { get; protected set; } = "DefaultConnection";
  }
}