using Joker.AspNetCore.MongoDb.Services;
using Joker.AspNetCore.MongoDb.Settings.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Joker.AspNetCore.MongoDb
{
  public class ApiStartup : Joker.OData.Startup.ApiStartup
  {
    public ApiStartup(IWebHostEnvironment env)
      : base(env)
    {
    }

    protected override void OnConfigureServices(IServiceCollection services)
    {
      base.OnConfigureServices(services);

      services.Configure<DatabaseSettings>(
        Configuration.GetSection(nameof(DatabaseSettings)));

      services.AddSingleton<IDatabaseSettings>(sp =>
        sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);

      services.AddSingleton<ICarMongoRepository, CarMongoRepository>();
    }
  }
}