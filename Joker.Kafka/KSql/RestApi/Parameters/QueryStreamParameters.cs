﻿using System.Collections.Generic;
using System.Text.Json.Serialization;
using Kafka.DotNet.ksqlDB.KSql.Query;
using Kafka.DotNet.ksqlDB.KSql.Query.Context;
using Kafka.DotNet.ksqlDB.KSql.Query.Options;

namespace Kafka.DotNet.ksqlDB.KSql.RestApi.Parameters
{
  public sealed class QueryStreamParameters : IQueryParameters
  {
    [JsonPropertyName("sql")]
    public string Sql { get; set; }

    [JsonPropertyName("properties")]
    public Dictionary<string, string> Properties { get; } = new();
    
    public static readonly string AutoOffsetResetPropertyName = "auto.offset.reset";

    public string this[string key]
    {
      get => Properties[key];
      set => Properties[key] = value;
    }

    internal QueryType QueryType { get; } = QueryType.QueryStream;
  }
}