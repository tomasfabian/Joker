using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace SqlTableDependency.Extensions.IntegrationTests
{
  public static class ObservableExtensions
  {
    public static Task<T> WaitFirst<T>(this IObservable<T> observable)
    {
      return observable.Timeout(TimeSpan.FromSeconds(2))
        .FirstOrDefaultAsync()
        .ToTask();
    }
  }
}