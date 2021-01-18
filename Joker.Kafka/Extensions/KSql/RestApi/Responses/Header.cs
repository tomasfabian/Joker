using System.Text.Json.Serialization;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Responses
{
  public class Header
  {
    [JsonPropertyName("queryId")]
    public string QueryId { get; set; }

    [JsonPropertyName("schema")]
    public string Schema { get; set; }
  }
}