using System.Collections.Generic;
using System.Threading;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi
{
  public interface IKSqldbProvider
  {
    IAsyncEnumerable<T> Run<T>(object parameters, CancellationToken cancellationToken = default);
  }
}