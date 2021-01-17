using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Joker.Kafka.Extensions.KSql.RestApi
{
  public sealed class KsqlQueryParameters
  {
    [JsonPropertyName("ksql")]
    public string KSql { get; set; }
      
    [JsonPropertyName("streamsProperties")]
    public Dictionary<string, string> StreamProperties { get; } = new();
  }
}