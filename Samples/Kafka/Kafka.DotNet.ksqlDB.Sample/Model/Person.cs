using Kafka.DotNet.ksqlDB.KSql.Query;

namespace Kafka.DotNet.ksqlDB.Sample.Model
{
  public class Person : Record
  {
    public string Name { get; set; }
  }
}