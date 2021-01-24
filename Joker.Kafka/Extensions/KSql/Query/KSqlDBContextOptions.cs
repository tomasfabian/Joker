using System;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Parameters;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  public sealed class KSqlDBContextOptions
  {
    public KSqlDBContextOptions(string url)
    {
      if(string.IsNullOrEmpty(url))
        throw new ArgumentNullException(nameof(url));

      Url = url;

      QueryStreamParameters = new QueryStreamParameters
      {
        ["auto.offset.reset"] = "earliest"
      };
    }    
    
    public string Url { get; }

    public QueryStreamParameters QueryStreamParameters { get; }
  }
}