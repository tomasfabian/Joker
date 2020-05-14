using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Sample.DataCore.EFCore
{
  public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SampleDbContextCore>
  {
    public SampleDbContextCore CreateDbContext(string[] args)
    {
      string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
      IConfigurationRoot configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{environment}.json", optional: true)
        .Build();
 
      var builder = new DbContextOptionsBuilder<SampleDbContextCore>();
 
      var connectionString = configuration.GetConnectionString("FargoEntities");
 
      builder.UseSqlServer(connectionString);
 
      return new SampleDbContextCore(builder.Options);
    }
  }
}