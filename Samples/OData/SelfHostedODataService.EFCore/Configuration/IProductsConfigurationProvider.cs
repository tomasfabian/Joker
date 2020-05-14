namespace SelfHostedODataService.EFCore.Configuration
{
  public interface IProductsConfigurationProvider
  {
    string GetDatabaseConnectionString();
    string RedisUrl { get; }
  }
}