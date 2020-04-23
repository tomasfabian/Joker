namespace SelfHostedODataService.Configuration
{
  public interface IProductsConfigurationProvider
  {
    string GetDatabaseConnectionString();
    string RedisUrl { get; }
  }
}