using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SqlTableDependency.Extensions.Tests.Models;
using SqlTableDependency.Extensions.Tests.SqlTableDependencies;
using TableDependency.SqlClient.Base.Abstracts;
using Should.Fluent;
using TableDependency.SqlClient.Base.EventArgs;

namespace SqlTableDependency.Extensions.Tests
{
  [TestClass]
  public class SqlTableDependencyProviderTests : TestBase
  {
    private Mock<ITableDependency<TestModel>> tableDependencyMoq;
    private string connectionString = @"TestConnection";

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      tableDependencyMoq = new Mock<ITableDependency<TestModel>>();
    }

    [TestMethod]
    public void SubscribeToEntityChanges_SuccessfulSubscription_StartWasCalled()
    {
      //Arrange
      var sqlDependencyProvider = CreateClassUnderTest();

      //Act
      sqlDependencyProvider.SubscribeToEntityChanges();
      
      //Assert
      tableDependencyMoq.Verify(c => c.Start(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
    }

    [TestMethod]
    public void OnError_TryReconnect_SuccessfulReconnectionAfterTimeSpan()
    {
      //Arrange
      var sqlDependencyProvider = CreateClassUnderTest();
      sqlDependencyProvider.SubscribeToEntityChanges();

      var errorEventArgs = CreateErrorEventArgs();

      //Act
      tableDependencyMoq.Raise(m => m.OnError += null, new[] { tableDependencyMoq.Object, errorEventArgs });

      TestScheduler.AdvanceBy(TimeSpan.FromSeconds(10).Ticks);

      //Assert
      tableDependencyMoq.Verify(c => c.Dispose(), Times.Once);
      tableDependencyMoq.Verify(c => c.Start(It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
    }

    [TestMethod]
    public void Dispose_TryStopLastConnection_DisposeWasCalled()
    {
      //Arrange
      var sqlDependencyProvider = CreateClassUnderTest();
      sqlDependencyProvider.SubscribeToEntityChanges();

      //Act
      sqlDependencyProvider.Dispose();

      //Assert
      tableDependencyMoq.Verify(c => c.Dispose(), Times.Once);
    }

    private TestSqlTableDependencyProvider CreateClassUnderTest()
    {
      return new TestSqlTableDependencyProvider(connectionString, TestScheduler, tableDependencyMoq.Object);
    }
    
    private static object CreateErrorEventArgs()
    {
      var args = new object[] { new Exception(), string.Empty, string.Empty, string.Empty };

      var errorEventArgs = typeof(ErrorEventArgs).Assembly.CreateInstance(
        typeof(ErrorEventArgs).FullName, false,
        BindingFlags.Instance | BindingFlags.NonPublic,
        null, args, null, null);

      return errorEventArgs;
    }

    [TestCleanup]
    public void CleanUp()
    {
      using (tableDependencyMoq.Object)
      {
      }
    }
  }
}
