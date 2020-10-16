using MongoDB.Bson.Serialization.Attributes;

namespace Colosseo.MagicPlatform.Models
{
  public class Car : DomainEntity
  {
    [BsonElement("Name")]
    public string CarName { get; set; }
  }
}