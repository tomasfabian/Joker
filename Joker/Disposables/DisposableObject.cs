using System;
using System.Reactive.Disposables;

namespace Joker.Disposables
{
  public class DisposableObject : ICompositeDisposable
  {
    private readonly object gate = new object();

    #region Properties

    public bool IsDisposed => compositeDisposable.IsDisposed;

    private CompositeDisposable compositeDisposable { get; } = new CompositeDisposable();

    #endregion

    #region Create

    public static DisposableObject Create(Action action)
    {
      return new Disposable(action);
    }

    #endregion
    
    #region AddDisposable

    public void AddDisposable(IDisposable item)
    {
      if (item == null)
      {
        throw new ArgumentNullException(nameof(item));
      }

      lock (gate)
      {
        if (!compositeDisposable.IsDisposed)
        {
          compositeDisposable.Add(item);
          return;
        }
      }

      item.Dispose();
    }

    #endregion

    #region RemoveDisposable

    public bool RemoveDisposable(IDisposable item)
    {
      if (item == null)
      {
        throw new ArgumentNullException(nameof(item));
      }

      lock (gate)
      {
        if (compositeDisposable.IsDisposed)
        {
          return false;
        }

        return compositeDisposable.Remove(item);
      }
    }

    #endregion

    #region Dispose

    public void Dispose()
    {
      Dispose(true);

      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      lock (gate)
      {
        if (!IsDisposed)
        {
          if (disposing)
          {
            compositeDisposable.Dispose();

            OnDispose();
          }
        }
      }
    }

    #endregion

    #region OnDispose

    protected virtual void OnDispose()
    {
    }

    #endregion

    private class Disposable : DisposableObject
    {
      private readonly Action dispose;

      public Disposable(Action dispose)
      {
        this.dispose = dispose ?? throw new ArgumentNullException(nameof(dispose));
      }

      protected override void OnDispose()
      {
        base.OnDispose();

        dispose();
      }
    }
  }
}