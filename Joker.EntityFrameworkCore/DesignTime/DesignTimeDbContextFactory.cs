using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Sample.DataCore.EFCore
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
 
      var connectionString = configuration.GetConnectionString("FargoEntities");
 
      builder.UseSqlServer(connectionString);

      return Create(builder.Options);
    }

    protected abstract TContext Create(DbContextOptions<TContext> options);
  }
}