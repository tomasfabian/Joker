using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Joker.Kafka.Extensions.KSql.RestApi
{
  public interface IKSqldbProvider<T>
  {
    IAsyncEnumerable<T> Run([EnumeratorCancellation] CancellationToken cancellationToken = default);
  }
}