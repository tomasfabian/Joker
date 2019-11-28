using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using Ninject.MockingKernel.Moq;
using SqlTableDependency.Extensions.Redis.SqlTableDependency;
using SqlTableDependency.Extensions.Tests;
using SqlTableDependency.Extensions.Tests.Models;

namespace SqlTableDependency.Extensions.Redis.Tests.SqlTableDependency
{
  [TestClass]
  public class SqlTableDependencyRedisProviderTests : TestBase
  {
    private readonly MoqMockingKernel mockingKernel = new MoqMockingKernel();
    private Mock<ISqlTableDependencyRedisProvider<TestModel>> ClassUnderTest;
      
    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      ClassUnderTest = mockingKernel.GetMock<ISqlTableDependencyRedisProvider<TestModel>>();
    }
    
    [TestMethod]
    public void StartPublishing()
    {
      //Arrange

      ////Act
      ClassUnderTest.Object.StartPublishing();

      ////Assert
      ClassUnderTest.Verify(c => c.StartPublishing(), Times.Exactly(1));
    }
  }
}