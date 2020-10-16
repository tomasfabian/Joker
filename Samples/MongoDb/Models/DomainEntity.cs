using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Joker.AspNetCore.MongoDb.Models
{
  public class DomainEntity
  {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
  }
}