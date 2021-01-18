using System.Text.Json.Serialization;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Responses
{
  public class HeaderResponse
  {
    [JsonPropertyName("header")]
    public Header Header { get; set; }
  }
}