using System;

namespace SqlTableDependency.Extensions.Disposables
{
  public class DisposableObject : IDisposable
  {
    #region Properties

    public bool IsDisposed { get; private set; }

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
  }
}