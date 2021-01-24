using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;
using Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.RestApi;
using Kafka.DotNet.ksqlDB.Tests.Helpers;
using Kafka.DotNet.ksqlDB.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Linq
{
  public class City
  {
    public string RegionCode { get; set; }
  }

  [TestClass]
  [Ignore("NotImplemented")]
  public class QbservableGroupByExtensionsTests : TestBase
  {

    [TestMethod]
    public void GroupBy_BuildKSql_PrintsQuery()
    {
      //Arrange
      var grouping = new CitiesStreamSet()
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
    public CitiesStreamSet()
      : base(new QbservableProvider(TestParameters.KsqlDBUrl))
    {
    }

    public CitiesStreamSet(IKSqlQbservableProvider provider, Expression expression)
      : base(provider, expression)
    {
    }

    protected override async IAsyncEnumerable<City> GetTestValues()
    {
      yield return new City { RegionCode = "A1" };

      yield return new City { RegionCode = "B@" };

      yield return new City { RegionCode = "A1" };
      
      await Task.CompletedTask;
    } 
    
    public override IKSqlQueryGenerator CreateKSqlQueryGenerator()
    {
      var queryGenerator = MockExtensions.CreateKSqlQueryGenerator("Cities");

      return queryGenerator;
    }
  }
}