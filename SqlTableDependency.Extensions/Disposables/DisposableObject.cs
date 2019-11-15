using System;

namespace SqlTableDependency.Extensions.Disposables
{
  public class DisposableObject : IDisposable
  {
    #region Fields

    private bool isDisposed;

    #endregion

    #region Dispose

    public void Dispose()
    {
      Dispose(true);

      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!isDisposed)
      {
        if (disposing)
        {
          OnDispose();
        }

        isDisposed = true;
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