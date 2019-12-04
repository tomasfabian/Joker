using System.Reactive.Concurrency;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject.MockingKernel.Moq;

namespace SqlTableDependency.Extensions.Tests
{
  public class TestBase<TClassUnderTest> : TestBase
  {
    protected TClassUnderTest ClassUnderTest { get; set; }
  }

  public class TestBase
  {
    protected readonly MoqMockingKernel MockingKernel = new MoqMockingKernel();
    protected TestScheduler TestScheduler = new TestScheduler();

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
    }

    [TestInitialize]
    public virtual void TestInitialize()
    {
      MockingKernel.Bind<IScheduler>().ToConstant(TestScheduler);
    }

    [TestCleanup]
    public virtual void ClassCleanup()
    {
    }

    #region RunSchedulers

    public void RunSchedulers()
    {
      TestScheduler.Start();
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