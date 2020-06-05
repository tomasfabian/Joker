using Joker.Factories.Schedulers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;

//namespace Joker.WinUI3.Sample.Factories.Schedulers
namespace Joker.WPF.Sample.Factories.Schedulers
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
            dispatcher = DispatcherScheduler.Current;

          return dispatcher;
        }
      }
    }
  }
}