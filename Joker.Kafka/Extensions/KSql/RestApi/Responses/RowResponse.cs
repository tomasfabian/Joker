using System.Text.Json.Serialization;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Responses
{
  public class RowResponse
  {
    [JsonPropertyName("row")]
    public Row Row { get; set; }

    [JsonPropertyName("errorMessage")]
    public object ErrorMessage { get; set; }
  }
}