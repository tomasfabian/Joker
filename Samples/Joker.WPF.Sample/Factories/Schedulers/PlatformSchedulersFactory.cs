using System.Reactive.Concurrency;
using Joker.Factories.Schedulers;
using Joker.Platforms.Factories.Schedulers;

namespace Joker.WPF.Sample.Factories.Schedulers
{
  public class PlatformSchedulersFactory : SchedulersFactory, IPlatformSchedulersFactory
  {
    #region Dispatcher

    private readonly object dispatcherGate = new object();

    private IScheduler dispatcher;

    public IScheduler Dispatcher
    {
      get
      {
        lock (dispatcherGate)
        {
          if (dispatcher == null)
            dispatcher = DispatcherScheduler.Current;

          return dispatcher;
        }
      }
    }

    #endregion
  }
}