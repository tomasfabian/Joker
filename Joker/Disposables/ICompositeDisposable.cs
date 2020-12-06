using System;

namespace Joker.Disposables
{
  public interface ICompositeDisposable : IDisposable
  {
    void AddDisposable(IDisposable item);
    bool RemoveDisposable(IDisposable item);
  }
}