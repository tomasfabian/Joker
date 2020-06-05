using System;
using System.Reactive.Concurrency;

//namespace Joker.WinUI3.Sample.Factories.Schedulers
namespace Joker.WPF.Sample.Factories.Schedulers
{
  public interface IPlatformSchedulersFactory
  {
    IScheduler Dispatcher { get;}
  }
}