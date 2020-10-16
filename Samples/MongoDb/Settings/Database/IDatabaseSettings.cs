namespace Joker.AspNetCore.MongoDb.Settings.Database
{
  public interface IDatabaseSettings
  {
    string CollectionName { get; set; }
    string ConnectionString { get; set; }
    string DatabaseName { get; set; }
  }
}