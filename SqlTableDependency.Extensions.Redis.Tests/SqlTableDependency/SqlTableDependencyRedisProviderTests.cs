using System;
using System.Reactive.Subjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using SqlTableDependency.Extensions.Notifications;
using SqlTableDependency.Extensions.Redis.ConnectionMultiplexers;
using SqlTableDependency.Extensions.Tests;
using SqlTableDependency.Extensions.Tests.Models;
using StackExchange.Redis;
using TableDependency.SqlClient.Base.Enums;

namespace SqlTableDependency.Extensions.Redis.Tests.SqlTableDependency
{
  [TestClass]
  public class SqlTableDependencyRedisProviderTests : TestBase<TestSqlTableDependencyRedisProvider>
  {
    private readonly ISubject<RecordChangedNotification<TestModel>> recordChangedSubject =
      new Subject<RecordChangedNotification<TestModel>>();

    private readonly ISubject<TableDependencyStatus> statusChangedSubject = new Subject<TableDependencyStatus>();

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      var sqlTableDependencyProvider = MockingKernel.GetMock<ISqlTableDependencyProvider<TestModel>>();

      sqlTableDependencyProvider.Setup(c => c.WhenEntityRecordChanges)
        .Returns(recordChangedSubject);

      sqlTableDependencyProvider.Setup(c => c.WhenStatusChanges)
        .Returns(statusChangedSubject);

      ClassUnderTest = MockingKernel.Get<TestSqlTableDependencyRedisProvider>();
    }

    [TestMethod]
    public void OnSqlTableDependencyRecordChanged()
    {
      //Arrange
      ClassUnderTest.StartPublishing();

      //Act
      recordChangedSubject.OnNext(new RecordChangedNotification<TestModel>());
      RunSchedulers();

      ////Assert
      MockingKernel.GetMock<IRedisPublisher>().Verify(c => c.PublishAsync(It.IsAny<RedisChannel>(), It.IsAny<RedisValue>(), CommandFlags.None), Times.Exactly(1));
    }

    [TestMethod]
    public void OnSqlTableDependencyStatusChanged()
    {
      //Arrange
      ClassUnderTest.StartPublishing();

      //Act
      statusChangedSubject.OnNext(TableDependencyStatus.Started);
      RunSchedulers();

      ////Assert
      MockingKernel.GetMock<IRedisPublisher>()
        .Verify(c => c.PublishAsync(It.IsAny<RedisChannel>(), It.IsAny<RedisValue>(), CommandFlags.None), Times.Exactly(1));
    }
  }
}