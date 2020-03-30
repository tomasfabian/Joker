using System;
using System.Reactive.Concurrency;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject.MockingKernel.Moq;
using UnitTests.Schedulers;

namespace UnitTests
{
  public class TestBase<TClassUnderTest> : TestBase
  {
    protected TClassUnderTest ClassUnderTest { get; set; }
  }

  public class TestBase
  {
    protected readonly MoqMockingKernel MockingKernel = new MoqMockingKernel();
    protected TestScheduler TestScheduler = new TestScheduler();

    protected ReactiveTestSchedulersFactory SchedulersFactory;

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
    }

    [TestInitialize]
    public virtual void TestInitialize()
    {
      SchedulersFactory = new ReactiveTestSchedulersFactory();

      MockingKernel.Bind<IScheduler>().ToConstant(TestScheduler);
      MockingKernel.Bind<ReactiveTestSchedulersFactory>().ToConstant(SchedulersFactory);
    }

    [TestCleanup]
    public virtual void ClassCleanup()
    {
    }

    #region RunSchedulers

    public void RunSchedulers()
    {
      TestScheduler.Start();
      
      SchedulersFactory.ThreadPool.Start();
      SchedulersFactory.TaskPool.Start();
      SchedulersFactory.Dispatcher.Start();
    }

    #endregion

    #region AdvanceSchedulers

    public void AdvanceSchedulers(long time)
    {
      TestScheduler.AdvanceBy(time);
    }

    #endregion    
    
    protected void Schedule(TestScheduler testScheduler, TimeSpan timeSpan, Action action)
    {
      testScheduler.Schedule(timeSpan, action);
    }
    
    protected void ScheduleOnTaskPool(TimeSpan timeSpan, Action action)
    {
      Schedule(SchedulersFactory.TaskPool, timeSpan, action);
    }
    
    protected void ScheduleOnThreadPool(TimeSpan timeSpan, Action action)
    {
      Schedule(SchedulersFactory.ThreadPool, timeSpan, action);
    }
  }
}