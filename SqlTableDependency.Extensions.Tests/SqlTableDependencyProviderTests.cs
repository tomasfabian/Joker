using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SqlTableDependency.Extensions.Tests.Models;
using SqlTableDependency.Extensions.Tests.SqlTableDependencies;
using TableDependency.SqlClient.Base.Abstracts;
using TableDependency.SqlClient.Base.EventArgs;

namespace SqlTableDependency.Extensions.Tests
{
  [TestClass]
  public class SqlTableDependencyProviderTests : TestBase
  {
    private Mock<ITableDependency<TestModel>> tableDependencyMoq;
    private string connectionString = @"TestConnection";

    private readonly TimeSpan testConnectionTimeStamp = TimeSpan.FromSeconds(10);

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      tableDependencyMoq = new Mock<ITableDependency<TestModel>>();
    }

    #region DontPreserveDatabaseObjects

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
      tableDependencyMoq.Raise(m => m.OnError += null, tableDependencyMoq.Object, errorEventArgs);

      TestScheduler.AdvanceBy(testConnectionTimeStamp.Ticks);

      //Assert
      tableDependencyMoq.Verify(c => c.Dispose(), Times.Once);
      tableDependencyMoq.Verify(c => c.Start(It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
    }

    [TestMethod]
    public void OnError_TryReconnect_ReconnectionAfterTimeSpanFailed()
    {
      SubscribeAndRaiseError();

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

    #endregion

    #region PreserveDatabaseObjects

    [TestMethod]
    [TestCategory("PreserveDatabaseObjects")]
    public void OnError_TryReconnectWithPreserveDatabaseObjects_SuccessfulReconnectionAfterTimeSpan()
    {
      SubscribeAndRaiseError(preserveDatabaseObjects: true);

      //Assert
      tableDependencyMoq.Verify(c => c.Dispose(), Times.Never);
      tableDependencyMoq.Verify(c => c.Start(It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
    }

    #endregion

    #region Helpers

    #region SubscribeAndRaiseError

    private void SubscribeAndRaiseError(bool preserveDatabaseObjects = false)
    {
      //Arrange
      var sqlDependencyProvider = CreateClassUnderTest(preserveDatabaseObjects);
      sqlDependencyProvider.IsDatabaseAvailableTestOverride = true;
      sqlDependencyProvider.SubscribeToEntityChanges();

      var errorEventArgs = CreateErrorEventArgs();

      //Act
      tableDependencyMoq.Raise(m => m.OnError += null, tableDependencyMoq.Object, errorEventArgs);

      TestScheduler.AdvanceBy(testConnectionTimeStamp.Ticks);
    }

    #endregion

    #region CreateClassUnderTest

    private TestSqlTableDependencyProvider CreateClassUnderTest(bool preserveDatabaseObjects = false)
    {
      return new TestSqlTableDependencyProvider(connectionString, TestScheduler, tableDependencyMoq.Object, preserveDatabaseObjects);
    }

    #endregion

    #region CreateErrorEventArgs

    private static object CreateErrorEventArgs()
    {
      var args = new object[] { new Exception(), string.Empty, string.Empty, string.Empty };

      var errorEventArgs = typeof(ErrorEventArgs).Assembly.CreateInstance(
        typeof(ErrorEventArgs).FullName, false,
        BindingFlags.Instance | BindingFlags.NonPublic,
        null, args, null, null);

      return errorEventArgs;
    }
    
    #endregion

    #endregion

    #region CleanUp

    [TestCleanup]
    public void CleanUp()
    {
      using (tableDependencyMoq.Object)
      {
      }
    }

    #endregion
  }
}