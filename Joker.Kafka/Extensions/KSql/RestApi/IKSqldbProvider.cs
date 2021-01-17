using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi
{
  public interface IKSqldbProvider<T>
  {
    IAsyncEnumerable<T> Run([EnumeratorCancellation] CancellationToken cancellationToken = default);
  }
}