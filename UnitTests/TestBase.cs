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

    protected ReactiveTestSchedulersFactory schedulersFactory;

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
    }

    [TestInitialize]
    public virtual void TestInitialize()
    {
      schedulersFactory = new ReactiveTestSchedulersFactory();

      MockingKernel.Bind<IScheduler>().ToConstant(TestScheduler);
      MockingKernel.Bind<ReactiveTestSchedulersFactory>().ToConstant(schedulersFactory);
    }

    [TestCleanup]
    public virtual void ClassCleanup()
    {
    }

    #region RunSchedulers

    public void RunSchedulers()
    {
      TestScheduler.Start();
      
      schedulersFactory.ThreadPool.Start();
      schedulersFactory.TaskPool.Start();
      schedulersFactory.Dispatcher.Start();
    }

    #endregion

    #region AdvanceSchedulers

    public void AdvanceSchedulers(long time)
    {
      TestScheduler.AdvanceBy(time);
    }

    #endregion
  }
}