using System.Reactive.Concurrency;
using Joker.Factories.Schedulers;

namespace Joker.WPF.Sample.Factories.Schedulers
{
  public class WpfSchedulersFactory : SchedulersFactory, IWpfSchedulersFactory
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