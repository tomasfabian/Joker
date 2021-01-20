using System.Collections.Generic;
using System.Threading;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi
{
  public interface IKSqldbProvider<T>
  {
    IAsyncEnumerable<T> Run(object parameters, CancellationToken cancellationToken = default);
  }
}