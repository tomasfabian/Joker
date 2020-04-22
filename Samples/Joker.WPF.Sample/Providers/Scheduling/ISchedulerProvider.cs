using System.Reactive.Concurrency;

namespace Joker.WPF.Sample.Providers.Scheduling
{
  public interface ISchedulerProvider
  {
    IScheduler Dispatcher { get; }
    IScheduler ThreadPool { get; }
    IScheduler TaskPool { get; }
  }
}