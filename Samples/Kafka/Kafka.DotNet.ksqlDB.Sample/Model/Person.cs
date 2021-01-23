using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;

namespace Kafka.DotNet.ksqlDB.Sample.Model
{
  public class Person : Record
  {
    public string Name { get; set; }
  }
}