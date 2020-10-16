namespace Joker.AspNetCore.MongoDb.Settings.Database
{
  public class DatabaseSettings : IDatabaseSettings
  {
    public string CollectionName { get; set; }
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
  }
}