namespace SelfHostedODataService.Configuration
{
  public interface IConfigurationProvider
  {
    string GetDatabaseConnectionString();
    string RedisUrl { get; }
  }
}