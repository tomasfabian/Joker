using System;
using System.Reactive.Disposables;

namespace Joker.Extensions.Disposables
{
  public static class DisposableExtensions
  {
    public static void DisposeWith(this IDisposable disposable, CompositeDisposable compositeDisposable)
    {
      if (disposable == null) throw new ArgumentNullException(nameof(disposable));
      if (compositeDisposable == null) throw new ArgumentNullException(nameof(compositeDisposable));

      compositeDisposable.Add(disposable);
    }
  }
}