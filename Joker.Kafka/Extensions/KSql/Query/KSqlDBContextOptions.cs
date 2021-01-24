using System;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  public sealed class KSqlDBContextOptions
  {
    public KSqlDBContextOptions(string url)
    {
      if(string.IsNullOrEmpty(url))
        throw new ArgumentNullException(nameof(url));

      Url = url;
    }    
    
    public string Url { get; }
  }
}