using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace SqlTableDependency.Extensions.IntegrationTests
{
  public static class ObservableExtensions
  {
    public static Task<T> WaitFirst<T>(this IObservable<T> observable, TimeSpan? timeout = null)
    {
      if (!timeout.HasValue)
        timeout = TimeSpan.FromSeconds(3);

      return observable.Timeout(timeout.Value)
        .FirstOrDefaultAsync()
        .ToTask();
    }
  }
}