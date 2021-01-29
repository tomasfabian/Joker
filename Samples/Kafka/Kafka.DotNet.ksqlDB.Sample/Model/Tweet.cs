using System.Text.Json.Serialization;

namespace Kafka.DotNet.ksqlDB.Sample.Model
{
  public class Tweet : Record
  {
    public int Id { get; set; }

    [JsonPropertyName("MESSAGE")]
    public string Message { get; set; }
  }
}