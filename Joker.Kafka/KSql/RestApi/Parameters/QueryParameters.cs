using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kafka.DotNet.ksqlDB.KSql.RestApi.Parameters
{
  public sealed class QueryParameters
  {
    [JsonPropertyName("ksql")]
    public string KSql { get; set; }

    [JsonPropertyName("streamsProperties")]
    public Dictionary<string, string> StreamsProperties { get; } = new();

    public static readonly string AutoOffsetResetPropertyName = "ksql.streams.auto.offset.reset";

    public string this[string key]
    {
      get => StreamsProperties[key];
      set => StreamsProperties[key] = value;
    }
  }
}