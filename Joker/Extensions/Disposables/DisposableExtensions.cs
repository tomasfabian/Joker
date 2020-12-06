using System;
using System.Reactive.Disposables;
using Joker.Disposables;

namespace Joker.Extensions.Disposables
{
  public static class DisposableExtensions
  {
    public static void DisposeWith(this IDisposable disposable, ICompositeDisposable compositeDisposable)
    {      
      if (disposable == null) throw new ArgumentNullException(nameof(disposable));
      if (compositeDisposable == null) throw new ArgumentNullException(nameof(compositeDisposable));

      compositeDisposable.AddDisposable(disposable);
    }

    public static void DisposeWith(this IDisposable disposable, CompositeDisposable compositeDisposable)
    {
      if (disposable == null) throw new ArgumentNullException(nameof(disposable));
      if (compositeDisposable == null) throw new ArgumentNullException(nameof(compositeDisposable));

      compositeDisposable.Add(disposable);
    }
  }
}