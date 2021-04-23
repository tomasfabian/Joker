using System.Collections.Generic;

namespace Kafka.DotNet.ksqlDB.KSql.Query.Context
{
  public class QueryContext
  {
    public string StreamName { get; internal set; }

    internal IDictionary<string, object> PropertyBag { get; set; } = new Dictionary<string, object>();
  }
}