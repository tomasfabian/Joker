using System.Text.Json.Serialization;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;

namespace Kafka.DotNet.ksqlDB.Sample.Model
{
  public class Tweet : Record
  {
    public int Id { get; set; }

    [JsonPropertyName("MESSAGE")]
    public string Message { get; set; }
  }
}