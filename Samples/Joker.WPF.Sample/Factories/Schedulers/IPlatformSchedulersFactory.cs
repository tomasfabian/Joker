using System.Reactive.Concurrency;
using Joker.Factories.Schedulers;

namespace Joker.WPF.Sample.Factories.Schedulers
{
  public interface IPlatformSchedulersFactory : ISchedulersFactory
  {
    IScheduler Dispatcher { get; }
  }
}