using System.Reactive.Concurrency;
using Joker.Factories.Schedulers;

namespace SqlTableDependency.Extensions.WPFSample.Factories.Schedulers
{
  public interface IWpfSchedulersFactory : ISchedulersFactory
  {
    IScheduler Dispatcher { get; }
  }
}