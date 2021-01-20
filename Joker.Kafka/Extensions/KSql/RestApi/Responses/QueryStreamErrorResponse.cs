using System.Text.Json.Serialization;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Responses
{
  public class QueryStreamErrorResponse
  {
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("error_code")]
    public int ErrorCode { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }
  }
}