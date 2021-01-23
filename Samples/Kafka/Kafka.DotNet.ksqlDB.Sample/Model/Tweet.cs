using System.Text.Json.Serialization;
//using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;

namespace Kafka.DotNet.ksqlDB.Sample.Model
{
  public class Tweet //: Record not released in nuget package
  {    
    public long RowTime { get; set; }

    public int Id { get; set; }

    [JsonPropertyName("MESSAGE")]
    public string Message { get; set; }
  }
}