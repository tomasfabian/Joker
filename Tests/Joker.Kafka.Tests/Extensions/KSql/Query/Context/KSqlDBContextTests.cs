using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using FluentAssertions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query.Context;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Parameters;
using Kafka.DotNet.ksqlDB.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnitTests;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Query.Context
{
  [TestClass]
  public class KSqlDBContextTests : TestBase
  {
    [TestMethod]
    public void CreateStreamSet_Subscribe_KSqldbProvidersRunWasCalled()
    {
      //Arrange
      var context = new TestableDbProvider<string>(TestParameters.KsqlDBUrl);

      //Act
      var streamSet = context.CreateStreamSet<string>().Subscribe(_ => {});

      //Assert
      streamSet.Should().NotBeNull();
      context.KSqldbProviderMock.Verify(c => c.Run<string>(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public void CreateStreamSet_Subscribe_QueryOptionsWereTakenFromContext()
    {
      //Arrange
      var contextOptions = new KSqlDBContextOptions(TestParameters.KsqlDBUrl);
      contextOptions.QueryStreamParameters["auto.offset.reset"] = "latest";

      var context = new TestableDbProvider<string>(contextOptions);

      //Act
      var subscription = context.CreateStreamSet<string>().Subscribe(_ => {});

      //Assert
      context.KSqldbProviderMock.Verify(c => c.Run<string>(It.Is<QueryStreamParameters>(c => c["auto.offset.reset"] == "latest"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public void CreateStreamSet_CalledMultipleTimes_KSqlQueryGeneratorBuildKSqlWasNotCalled()
    {
      //Arrange
      var context = new TestableDbProvider<string>(TestParameters.KsqlDBUrl);

      //Act
      var subscription = context.CreateStreamSet<string>().Subscribe(_ => {});

      //Assert
      context.KSqlQueryGenerator.Verify(c => c.BuildKSql(It.IsAny<Expression>(), It.IsAny<QueryContext>()), Times.Once);
    }

    [TestMethod]
    public void CreateStreamSet_Subscribe_KSqlQueryGenerator()
    {
      //Arrange
      var contextOptions = new KSqlDBContextOptions(TestParameters.KsqlDBUrl);
      contextOptions.QueryStreamParameters["auto.offset.reset"] = "latest";

      var context = new TestableDbProvider<string>(contextOptions);

      //Act
      var subscription = context.CreateStreamSet<string>().Subscribe(_ => {}, e => {});

      //Assert
      context.KSqldbProviderMock.Verify(c => c.Run<string>(It.Is<QueryStreamParameters>(parameters => parameters["auto.offset.reset"] == "latest"), It.IsAny<CancellationToken>()), Times.Once);
    }

    private class TestableDbProvider<TValue> : KSqlDBContext
    {
      public TestableDbProvider(string ksqlDbUrl) : base(ksqlDbUrl)
      {
        InitMocks();
      }

      public TestableDbProvider(KSqlDBContextOptions contextOptions) : base(contextOptions)
      {
        InitMocks();
      }

      private void InitMocks()
      {
        KSqldbProviderMock.Setup(c => c.Run<TValue>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
          .Returns(GetEmptyAsyncEnumerable);
      }

      private static IAsyncEnumerable<TValue> GetEmptyAsyncEnumerable()
      {
        return new List<TValue>().ToAsyncEnumerable();
      }

      public readonly Mock<IKSqlDbProvider> KSqldbProviderMock = new Mock<IKSqlDbProvider>();
      public readonly Mock<IKSqlQueryGenerator> KSqlQueryGenerator = new Mock<IKSqlQueryGenerator>();

      protected override void OnConfigureServices(IServiceCollection serviceCollection)
      {
        serviceCollection.AddSingleton(KSqldbProviderMock.Object);
        serviceCollection.AddSingleton(KSqlQueryGenerator.Object);
      }
    }
  }
}