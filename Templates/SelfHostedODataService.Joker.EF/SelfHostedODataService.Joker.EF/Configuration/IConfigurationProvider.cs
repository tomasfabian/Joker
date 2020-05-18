namespace SelfHostedODataService.Joker.EF.Configuration
{
  public interface IConfigurationProvider
  {
    string GetDatabaseConnectionString();
    string RedisUrl { get; }
  }
}