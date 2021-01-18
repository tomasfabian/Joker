using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Parameters
{
  public sealed class KsqlQueryParameters
  {
    [JsonPropertyName("ksql")]
    public string KSql { get; set; }

    [JsonPropertyName("streamsProperties")]
    public Dictionary<string, string> StreamsProperties { get; } = new();

    public string this[string key]
    {
      get => StreamsProperties[key];
      set => StreamsProperties[key] = value;
    }
  }
}