using System;
using System.Net.Http;
using System.Threading.Tasks;
using Kafka.DotNet.ksqlDB.KSql.Query;

namespace Kafka.DotNet.ksqlDB.KSql.Linq
{
  public static class CreateStatementsExtensions
  {
    #region ExecuteStatementAsync

    public static Task<HttpResponseMessage> ExecuteStatementAsync<TSource>(this ICreateStatement<TSource> source)
    {
      if (source == null) throw new ArgumentNullException(nameof(source));

      var kStreamSet = source as KStreamSet<TSource>;

      return kStreamSet?.ExecuteAsync();
    }

    #endregion
  }
}