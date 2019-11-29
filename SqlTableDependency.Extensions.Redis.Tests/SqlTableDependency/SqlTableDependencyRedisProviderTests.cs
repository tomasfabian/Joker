using System;
using System.Reactive.Subjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using Ninject.MockingKernel.Moq;
using SqlTableDependency.Extensions.Redis.SqlTableDependency;
using SqlTableDependency.Extensions.Tests;
using SqlTableDependency.Extensions.Tests.Models;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;

namespace SqlTableDependency.Extensions.Redis.Tests.SqlTableDependency
{
  [TestClass]
  public class SqlTableDependencyRedisProviderTests : TestBase
  {
    private readonly MoqMockingKernel mockingKernel = new MoqMockingKernel();
    private Mock<ISqlTableDependencyRedisProvider<TestModel>> ClassUnderTest;

    private readonly ISubject<RecordChangedEventArgs<TestModel>> recordChangedSubject =
      new Subject<RecordChangedEventArgs<TestModel>>();

    private readonly ISubject<TableDependencyStatus> statusChangedSubject = new Subject<TableDependencyStatus>();

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      var sqlTableDependencyProvider = mockingKernel.GetMock<ISqlTableDependencyProvider<TestModel>>();

      sqlTableDependencyProvider.Setup(c => c.WhenEntityRecordChanges)
        .Returns(recordChangedSubject);

      sqlTableDependencyProvider.Setup(c => c.WhenStatusChanges)
        .Returns(statusChangedSubject);

      ClassUnderTest = mockingKernel.GetMock<ISqlTableDependencyRedisProvider<TestModel>>();
    }

    [TestMethod]
    public void StartPublishing()
    {
      //Arrange

      //Act
      ClassUnderTest.Object.StartPublishing();

      ////Assert
      ClassUnderTest.Verify(c => c.StartPublishing(), Times.Exactly(1));
    }
  }
}