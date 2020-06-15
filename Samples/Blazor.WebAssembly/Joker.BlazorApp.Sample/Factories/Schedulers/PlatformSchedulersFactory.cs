using System.Reactive.Concurrency;
using Joker.Factories.Schedulers;
using Joker.Platforms.Factories.Schedulers;

namespace Joker.BlazorApp.Sample.Factories.Schedulers
{
  public class PlatformSchedulersFactory : SchedulersFactory, IPlatformSchedulersFactory
  {
    public PlatformSchedulersFactory()
    {
      Dispatcher = CurrentThread; //TODO: is there a renderer thread?
    }

    public IScheduler Dispatcher { get; }
  }
}