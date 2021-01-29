using System;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Parameters;

namespace Kafka.DotNet.ksqlDB.KSql.Query.Context
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

    public bool ShouldPluralizeStreamName { get; set; } = true;

    public string Url { get; }

    public QueryStreamParameters QueryStreamParameters { get; }
  }
}