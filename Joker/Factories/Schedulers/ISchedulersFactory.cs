using System.Reactive.Concurrency;

namespace Joker.Factories.Schedulers
{
  public interface ISchedulersFactory
  {
    IScheduler CurrentThread { get; }
    IScheduler Immediate { get; }
    IScheduler NewThread { get; }
    IScheduler TaskPool { get; }
    IScheduler ThreadPool { get; }
    IScheduler EventLoopScheduler { get; }
    IScheduler NewEventLoopScheduler { get; }
  }
}