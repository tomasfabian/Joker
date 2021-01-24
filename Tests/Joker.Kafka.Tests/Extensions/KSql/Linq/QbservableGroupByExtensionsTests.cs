using FluentAssertions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;
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
    [TestMethod]
    public void GroupBy_BuildKSql_PrintsQuery()
    {
      //Arrange
      var dependencies = new TestKStreamSetDependencies
      {
        KSqlQueryGenerator = new CitiesKSqlQueryGenerator()
        //KSqlQueryGenerator = MockExtensions.CreateKSqlQueryGenerator("Cities")
      };

      var grouping = new CitiesStreamSet(dependencies)
          .GroupBy(c => c.RegionCode);

      //Act
      var ksql = grouping.ToQueryString();

      //Assert
      ksql.Should().BeEquivalentTo(@$"SELECT * FROM Cities GROUP BY 'RegionCode' EMIT CHANGES;");

      //SELECT 'RegionCode', COUNT(*) FROM
      //  Cities GROUP BY 'RegionCode' EMIT CHANGES;
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

  public class CitiesKSqlQueryGenerator : KSqlQueryGenerator
  {
    protected override string InterceptStreamName(string value)
    {
      return "Cities";
    }
  }
}