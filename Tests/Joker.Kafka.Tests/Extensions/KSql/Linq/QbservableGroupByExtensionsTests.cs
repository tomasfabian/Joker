using FluentAssertions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using UnitTests;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Linq
{
  public class City
  {
    public string RegionCode { get; set; }
  }

  [TestClass]
  public class QbservableGroupByExtensionsTests : TestBase
  {
    private CitiesStreamSet CreateQbservable()
    {
      var dependencies = new TestKStreamSetDependencies();

      dependencies.KSqldbProviderMock.Setup(c => c.Run<int>(It.IsAny<object>(), It.IsAny<CancellationToken>())).Returns(GetTestValues);

      return new CitiesStreamSet(dependencies);
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

    public static async IAsyncEnumerable<int> GetTestValues()
    {
      yield return 1;

      yield return 2;
      
      yield return 3;
      
      await Task.CompletedTask;
    }
  }

  internal class CitiesStreamSet : TestableKStreamSet<City>
  {
    public CitiesStreamSet(TestKStreamSetDependencies dependencies)
      : base(dependencies)
    {
    }

    protected override async IAsyncEnumerable<City> GetTestValues()
    {
      yield return new City { RegionCode = "A1" };

      yield return new City { RegionCode = "B@" };

      yield return new City { RegionCode = "A1" };
      
      await Task.CompletedTask;
    }
  }
}