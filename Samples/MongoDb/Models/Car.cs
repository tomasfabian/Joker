using MongoDB.Bson.Serialization.Attributes;

namespace Joker.AspNetCore.MongoDb.Models
{
  public class Car : DomainEntity
  {
    [BsonElement("Name")]
    public string CarName { get; set; }
  }
}