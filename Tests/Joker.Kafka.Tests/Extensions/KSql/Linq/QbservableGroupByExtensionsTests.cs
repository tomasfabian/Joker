﻿using FluentAssertions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query.Context;
using Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Query.Context;
using Kafka.DotNet.ksqlDB.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UnitTests;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Linq
{
  [TestClass]
  public class QbservableGroupByExtensionsTests : TestBase
  {
    private IQbservable<City> CreateQbservable()
    {
      var context = new TestableDbProvider(TestParameters.KsqlDBUrl);
      
      context.KSqldbProviderMock.Setup(c => c.Run<int>(It.IsAny<object>(), It.IsAny<CancellationToken>())).Returns(GetTestValues);
      context.KSqldbProviderMock.Setup(c => c.Run<long>(It.IsAny<object>(), It.IsAny<CancellationToken>())).Returns(GetDecimalTestValues);

      return context.CreateStreamSet<City>();
    }

    [TestMethod]
    public void GroupByAndCount_BuildKSql_PrintsQuery()
    {
      //Arrange
      var grouping = CreateQbservable()
          .GroupBy(c => c.RegionCode)
          .Select(g => g.Count());

      //Act
      var ksql = grouping.ToQueryString();

      //Assert
      ksql.Should().BeEquivalentTo("SELECT COUNT(*) FROM Cities GROUP BY RegionCode EMIT CHANGES;");
    }

    [TestMethod]
    public void GroupByAndCount_Named_BuildKSql_PrintsQuery()
    {
      //Arrange
      var grouping = CreateQbservable()
        .GroupBy(c => c.RegionCode)
        .Select(g => new {Count = g.Count()});     

      //Act
      var ksql = grouping.ToQueryString();

      //Assert
      ksql.Should().BeEquivalentTo("SELECT COUNT(*) Count FROM Cities GROUP BY RegionCode EMIT CHANGES;");
    }

    [TestMethod]
    public void GroupByAndCountByKey_BuildKSql_PrintsQuery()
    {
      //Arrange
      var grouping = CreateQbservable()
          .GroupBy(c => c.RegionCode)
          .Select(g => new { RegionCode = g.Key, Count = g.Count()});

      //Act
      var ksql = grouping.ToQueryString();

      //Assert
      ksql.Should().BeEquivalentTo("SELECT RegionCode, COUNT(*) Count FROM Cities GROUP BY RegionCode EMIT CHANGES;");
    }

    [TestMethod]
    public void GroupByAndCount_Subscribe_ReceivesValues()
    {
      //Arrange
      var grouping = CreateQbservable()
          .GroupBy(c => c.RegionCode)
          .Select(g => g.Count());

      bool valuesWereReceived = false;

      //Act
      var subscription = grouping.Subscribe(c => { valuesWereReceived = true; });

      //Assert
      valuesWereReceived.Should().BeTrue();

      subscription.Dispose();
    }

    [TestMethod]
    public void GroupByAndSum_Subscribe_ReceivesValues()
    {
      //Arrange
      var grouping = CreateQbservable()
        .GroupBy(c => c.RegionCode)
        .Select(g => g.Sum(c => c.Citizens));

      bool valuesWereReceived = false;

      //Act
      var subscription = grouping.Subscribe(c => { valuesWereReceived = true; });

      //Assert
      valuesWereReceived.Should().BeTrue();

      subscription.Dispose();
    }

    [TestMethod]
    public void GroupByAndSum_BuildKSql_PrintsQuery()
    {
      //Arrange
      var grouping = CreateQbservable()
        .GroupBy(c => c.RegionCode)
        .Select(g => g.Sum(c => c.Citizens));

      //Act
      var ksql = grouping.ToQueryString();

      //Assert
      ksql.Should().BeEquivalentTo("SELECT SUM(Citizens) FROM Cities GROUP BY RegionCode EMIT CHANGES;");
    }

    [TestMethod]
    public void GroupByAndSumWithColumn_BuildKSql_PrintsQuery()
    {
      //Arrange
      var grouping = CreateQbservable()
        .GroupBy(c => c.RegionCode)
        .Select(g => new { RegionCode = g.Key, MySum = g.Sum(c => c.Citizens)});

      //Act
      var ksql = grouping.ToQueryString();

      //Assert
      ksql.Should().BeEquivalentTo("SELECT RegionCode, SUM(Citizens) MySum FROM Cities GROUP BY RegionCode EMIT CHANGES;");
    }

    public static async IAsyncEnumerable<int> GetTestValues()
    {
      yield return 1;

      yield return 2;
      
      yield return 3;
      
      await Task.CompletedTask;
    }

    public static async IAsyncEnumerable<long> GetDecimalTestValues()
    {
      yield return 1;

      yield return 2;
      
      yield return 3;
      
      await Task.CompletedTask;
    }  

    protected async IAsyncEnumerable<City> GetCities()
    {
      yield return new City { RegionCode = "A1" };

      yield return new City { RegionCode = "B1" };

      yield return new City { RegionCode = "A1" };
      
      await Task.CompletedTask;
    }

    public class City
    {
      public string RegionCode { get; set; }
      public long Citizens { get; set; }
    }
  }

  class TestableDbProvider : TestableDbProvider<QbservableGroupByExtensionsTests.City>
  {
    public TestableDbProvider(string ksqlDbUrl) : base(ksqlDbUrl)
    {
    }

    public TestableDbProvider(KSqlDBContextOptions contextOptions) : base(contextOptions)
    {
    }

    protected override void OnConfigureServices(IServiceCollection serviceCollection)
    {
      serviceCollection.AddSingleton(KSqldbProviderMock.Object);
    }
  }
}