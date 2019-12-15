using System.Reactive.Concurrency;

namespace SqlTableDependency.Extensions.WPFSample.Providers.Scheduling
{
  public interface ISchedulerProvider
  {
    IScheduler Dispatcher { get; }
    IScheduler ThreadPool { get; }
    IScheduler TaskPool { get; }
  }
}