using System.Reactive.Concurrency;
using Joker.Factories.Schedulers;

namespace Joker.Platforms.Factories.Schedulers
{
  public interface IPlatformSchedulersFactory : ISchedulersFactory
  {
    IScheduler Dispatcher { get; }
  }
}