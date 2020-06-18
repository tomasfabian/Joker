using System.Reactive.Concurrency;
using Joker.Factories.Schedulers;
using Joker.Platforms.Factories.Schedulers;

namespace Joker.WinUI3.UWP.Sample.Factories.Schedulers
{
  public class PlatformSchedulersFactory : SchedulersFactory, IPlatformSchedulersFactory
  {
    private readonly object dispatcherGate = new object();

    private IScheduler dispatcher;

    public IScheduler Dispatcher
    {
      get
      {
        lock (dispatcherGate)
        {
          if (dispatcher == null)
            dispatcher = new CoreDispatcherScheduler(Microsoft.UI.Xaml.Window.Current.Dispatcher);

          return dispatcher;
        }
      }
    }
  }
}