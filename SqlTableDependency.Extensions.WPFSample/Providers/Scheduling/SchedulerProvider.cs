using System;
using System.Reactive.Concurrency;

namespace SqlTableDependency.Extensions.WPFSample.Providers.Scheduling
{
  public class SchedulerProvider : ISchedulerProvider
  {
    #region Dispatcher

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

    #endregion

    #region ThreadPool

    private readonly object threadPoolGate = new object();

    private IScheduler threadPool;

    public IScheduler ThreadPool
    {
      get
      {
        lock (threadPoolGate)
        {

          if (threadPool == null)
            threadPool = ThreadPoolScheduler.Instance;

          return threadPool;
        }
      }
    }

    #endregion

    #region TaskPool

    private readonly object taskPoolGate = new object();

    private IScheduler taskPool;

    public IScheduler TaskPool
    {
      get
      {
        lock (taskPoolGate)
        {

          if (taskPool == null)
            taskPool = TaskPoolScheduler.Default;

          return taskPool;
        }
      }
    }

    #endregion
  }
}