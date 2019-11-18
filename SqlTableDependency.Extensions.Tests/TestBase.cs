using System.Reactive.Concurrency;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SqlTableDependency.Extensions.Tests
{
  public class TestBase
  {
    protected TestScheduler TestScheduler = new TestScheduler();

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
    }

    [TestInitialize]
    public virtual void TestInitialize()
    {
    }

    [TestCleanup]
    public virtual void ClassCleanup()
    {
    }
  }
}