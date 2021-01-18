using System.Text.Json.Serialization;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Responses
{
  public class EndResponse
  {
    [JsonPropertyName("finalMessage")]
    public string FinalMessage { get; set; }
  }
}