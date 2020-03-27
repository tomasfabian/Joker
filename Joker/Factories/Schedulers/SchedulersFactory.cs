using System;
using System.Reactive.Concurrency;

namespace Joker.Factories.Schedulers
{
  public class SchedulersFactory : ISchedulersFactory
  {
    #region CurrentThread

    private readonly object currentThreadGate = new object();

    private IScheduler currentThread;

    public IScheduler CurrentThread
    {
      get
      {
        lock (currentThreadGate)
        {
          if (currentThread == null)
            currentThread = CurrentThreadScheduler.Instance.Catch<Exception>(OnCatch);

          return currentThread;
        }
      }
    }

    #endregion

    #region Immediate

    private readonly object immediateGate = new object();

    private IScheduler immediate;

    public IScheduler Immediate
    {
      get
      {
        lock (immediateGate)
        {

          if (immediate == null)
            immediate = ImmediateScheduler.Instance.Catch<Exception>(OnCatch);

          return immediate;
        }
      }
    }

    #endregion

    #region NewThread

    private readonly object newThreadGate = new object();

    private IScheduler newThread;

    public IScheduler NewThread
    {
      get
      {
        lock (newThreadGate)
        {

          if (newThread == null)
            newThread = NewThreadScheduler.Default.Catch<Exception>(OnCatch);

          return newThread;
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
            taskPool = TaskPoolScheduler.Default.Catch<Exception>(OnCatch);

          return taskPool;
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
            threadPool = ThreadPoolScheduler.Instance.Catch<Exception>(OnCatch);

          return threadPool;
        }
      }
    }

    #endregion

    #region EventLoopScheduler

    private readonly object eventLoopGate = new object();

    private IScheduler eventLoopScheduler;

    public IScheduler EventLoopScheduler
    {
      get
      {
        lock (eventLoopGate)
        {
          if (eventLoopScheduler == null)
            eventLoopScheduler = new EventLoopScheduler().Catch<Exception>(OnCatch);

          return eventLoopScheduler;
        }
      }
    }

    #endregion

    #region NewEventLoopScheduler

    public IScheduler NewEventLoopScheduler => new EventLoopScheduler();

    #endregion

    #region Methods

    public static Action<Exception> OnError { get; set; }

    protected bool OnCatch(Exception error)
    {
      OnError?.Invoke(error);

      return false;
    }

    #endregion
  }
}