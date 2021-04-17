using System.Text.Json.Serialization;

namespace Kafka.DotNet.ksqlDB.KSql.RestApi.Statements
{
  internal class KSqlStatement
  {
    public string ksql { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? commandSequenceNumber { get; set; }
  }
}