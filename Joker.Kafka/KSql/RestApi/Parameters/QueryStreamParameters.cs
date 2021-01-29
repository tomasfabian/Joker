using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Parameters
{
  public sealed class QueryStreamParameters
  {
    [JsonPropertyName("sql")]
    public string Sql { get; set; }

    [JsonPropertyName("properties")]
    public Dictionary<string, string> Properties { get; } = new();

    public string this[string key]
    {
      get => Properties[key];
      set => Properties[key] = value;
    }
  }
}