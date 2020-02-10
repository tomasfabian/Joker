using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SqlTableDependency.Extensions.Enums;
using SqlTableDependency.Extensions.Notifications;
using SqlTableDependency.Extensions.Tests.Models;
using SqlTableDependency.Extensions.Tests.SqlTableDependencies;
using TableDependency.SqlClient.Base;
using TableDependency.SqlClient.Base.Abstracts;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;
using TableDependency.SqlClient.Base.Messages;
using TableDependency.SqlClient.Base.Utilities;

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
    public void OnError_TryReconnect_SuccessfulReconnectionAfterSecondAttempt()
    {
      //Arrange
      var sqlDependencyProvider = CreateClassUnderTest();
      sqlDependencyProvider.SubscribeToEntityChanges();

      var errorEventArgs = CreateErrorEventArgs();

      //Act
      tableDependencyMoq.Raise(m => m.OnError += null, tableDependencyMoq.Object, errorEventArgs);
      sqlDependencyProvider.IsDatabaseAvailableTestOverride = false;
      TestScheduler.AdvanceBy(testConnectionTimeStamp.Ticks);
      sqlDependencyProvider.IsDatabaseAvailableTestOverride = true;
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

    [TestMethod]
    public void OnChanged_WhenEntityRecordChangesNotifies_Inserted()
    {
      //Arrange
      var sqlDependencyProvider = CreateClassUnderTest();
      sqlDependencyProvider.SubscribeToEntityChanges();

      RecordChangedNotification<TestModel> recordChangedNotification = null;

      var subscription = sqlDependencyProvider.WhenEntityRecordChanges.Subscribe(c => recordChangedNotification = c);

      var recordChangedEventArgsNotification = CreateRecordChangedEventArgsNotification(ChangeType.Insert);

      //Act
      tableDependencyMoq.Raise(m => m.OnChanged += null, tableDependencyMoq.Object, recordChangedEventArgsNotification);

      //Assert
      Assert.IsNotNull(recordChangedNotification);
      Assert.AreEqual(ChangeType.Insert, recordChangedNotification.ChangeType);

      subscription.Dispose();
    }

    [TestMethod]
    public void OnStatusChanged_WhenStatusChangesNotifies_TableDependencyStatus()
    {
      //Arrange
      var sqlDependencyProvider = CreateClassUnderTest();
      sqlDependencyProvider.SubscribeToEntityChanges();
      
      TableDependencyStatus tableDependencyStatus = TableDependencyStatus.None;

      var subscription = sqlDependencyProvider.WhenStatusChanges.Subscribe(c => tableDependencyStatus = c);

      StatusChangedEventArgs statusChangedEventArgs = CreateStatusChangedEventArgs();

      //Act
      tableDependencyMoq.Raise(m => m.OnStatusChanged += null, tableDependencyMoq.Object, statusChangedEventArgs);

      //Assert
      Assert.AreEqual(TableDependencyStatus.Started, tableDependencyStatus);

      subscription.Dispose();
    }

    #endregion

    #region ApplicationScope

    [TestMethod]
    [TestCategory("PreserveDatabaseObjects_ApplicationScope")]
    public void OnError_TryReconnect_ApplicationScope_SuccessfulReconnectionAfterTimeSpan()
    {
      SubscribeAndRaiseError(lifetimeScope: LifetimeScope.ApplicationScope);

      //Assert
      tableDependencyMoq.Verify(c => c.Dispose(), Times.Never);
      tableDependencyMoq.Verify(c => c.Start(It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
    }

    #endregion
    
    #region UniqueScope

    [TestMethod]
    [TestCategory("PreserveDatabaseObjects_UniqueScope")]
    public void OnError_TryReconnect_UniqueScope_SuccessfulReconnectionAfterTimeSpan()
    {
      SubscribeAndRaiseError(lifetimeScope: LifetimeScope.UniqueScope);

      //Assert
      tableDependencyMoq.Verify(c => c.Dispose(), Times.Never);
      tableDependencyMoq.Verify(c => c.Start(It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
    }

    #endregion

    #region Helpers

    #region SubscribeAndRaiseError

    private void SubscribeAndRaiseError(LifetimeScope lifetimeScope = LifetimeScope.ConnectionScope)
    {
      //Arrange
      var sqlDependencyProvider = CreateClassUnderTest(lifetimeScope);
      sqlDependencyProvider.IsDatabaseAvailableTestOverride = true;
      sqlDependencyProvider.SubscribeToEntityChanges();

      var errorEventArgs = CreateErrorEventArgs();

      //Act
      tableDependencyMoq.Raise(m => m.OnError += null, tableDependencyMoq.Object, errorEventArgs);

      TestScheduler.AdvanceBy(testConnectionTimeStamp.Ticks);
    }

    #endregion

    #region CreateClassUnderTest

    private TestSqlTableDependencyProvider CreateClassUnderTest(LifetimeScope lifetimeScope = LifetimeScope.ConnectionScope)
    {
      return new TestSqlTableDependencyProvider(connectionString, TestScheduler, tableDependencyMoq.Object, lifetimeScope);
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

    #region CreateStatusChangedEventArgs

    private static StatusChangedEventArgs CreateStatusChangedEventArgs()
    {
      var args = new object[] { TableDependencyStatus.Started,  "Server", "db", "sender" };

      var statusChangedEventArgs = typeof(ErrorEventArgs).Assembly.CreateInstance(
        typeof(StatusChangedEventArgs).FullName, false,
        BindingFlags.Instance | BindingFlags.NonPublic,
        null, args, null, null)  as StatusChangedEventArgs;
      
      return statusChangedEventArgs;
    }

    #endregion

    #region CreateRecordChangedEventArgsNotification

    private static RecordChangedEventArgs<TestModel> CreateRecordChangedEventArgsNotification(ChangeType changeType)
    {
      var schemaName = "dbo";
      var entityName = nameof(TestModel);
      var guid = "aef4d1af-ac7d-42d6-95dc-5034dafac81e";

      var messagesBag = CreateMessagesBag(null, new List<string> {$"{schemaName}_{entityName}_{guid}/{nameof(TestModel.Id)}"});

      messagesBag.AddMessage(new Message($"{schemaName}_{entityName}_{guid}/StartMessage/{changeType.ToString()}", null));

      var modelToTableMapper = new ModelToTableMapper<TestModel>();
      modelToTableMapper.AddMapping(c => c.Id, nameof(TestModel.Id));

      var recordChangedEventArgsNotification = new RecordChangedEventArgs<TestModel>(messagesBag: messagesBag,
        mapper: modelToTableMapper,
        userInterestedColumns: new List<TableColumnInfo>
        {
          new TableColumnInfo(nameof(TestModel.Id), "int"),
          new TableColumnInfo(nameof(TestModel.Name), "nvarchar", "256")
        },
        server: "dbName",
        database: @"localhost\SQLEXPRESS",
        sender: $"{schemaName}_{entityName}_{guid}",
        cultureInfo: new CultureInfo("en-US"));

      return recordChangedEventArgsNotification;
    }

    #endregion

    #region CreateMessagesBag

    private static MessagesBag CreateMessagesBag(
      Encoding encoding,
      ICollection<string> processableMessages)
    {
      var entityName = nameof(TestModel);
      var guid = "aef4d1af-ac7d-42d6-95dc-5034dafac81e";
      var dataBaseObjectsNamingConvention = $"dbo_{entityName}_{guid}";

      return new MessagesBag(encoding ?? Encoding.Unicode, new List<string>()
      {
        $"{dataBaseObjectsNamingConvention}/StartMessage/{ChangeType.Insert}",
        $"{dataBaseObjectsNamingConvention}/StartMessage/{ChangeType.Update}",
        $"{dataBaseObjectsNamingConvention}/StartMessage/{ChangeType.Delete}"
      }, $"{dataBaseObjectsNamingConvention}/EndMessage", processableMessages);
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