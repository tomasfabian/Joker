using System;

namespace Joker.Disposables
{
  public class DisposableObject : IDisposable
  {
    #region Properties

    public bool IsDisposed { get; private set; }

    #endregion

    #region Create

    public static DisposableObject Create(Action action)
    {
      return new Disposable(action);
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
      if (!IsDisposed)
      {
        if (disposing)
        {
          OnDispose();
        }

        IsDisposed = true;
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