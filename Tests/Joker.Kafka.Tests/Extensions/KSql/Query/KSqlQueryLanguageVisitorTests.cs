using FluentAssertions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;
using Location = Kafka.DotNet.ksqlDB.Tests.Models.Location;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Query
{
  [TestClass]
  public class KSqlQueryLanguageVisitorTests : TestBase<KSqlQueryLanguageVisitor<Models.Location>>
  {
    string streamName = nameof(Location);

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      ClassUnderTest = new KSqlQueryLanguageVisitor<Location>();
    }

    private IQbservable<Location> CreateStreamSource()
    {
      return new KStreamSet<Location>(new QbservableProvider());
    }

    #region Select

    [TestMethod]
    public void Select_BuildKSql_PrintsSelect()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(l => new {l.Longitude, l.Latitude});

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression);

      //Assert
      ksql.Should().BeEquivalentTo(@$"SELECT {nameof(Location.Longitude)}, {nameof(Location.Latitude)} FROM {streamName} EMIT CHANGES;");
    }
    
    [TestMethod]
    public void SelectWhere_BuildKSql_PrintsSelectFromWhere()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(l => new {l.Longitude, l.Latitude})
        .Where(p => p.Latitude == "1");

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression);

      //Assert
      string expectedKsql =
        @$"SELECT {nameof(Location.Longitude)}, {nameof(Location.Latitude)} FROM {streamName}
WHERE {nameof(Location.Latitude)} = '1' EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    [TestMethod]
    public void SelectMultipleWhere_BuildKSql_PrintsSelectFromWheres()
    {
      //Arrange
      var query = CreateStreamSource()
        .Where(p => p.Latitude == "1")
        .Where(p => p.Longitude == 0.1)
        .Select(l => new {l.Longitude, l.Latitude});

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression);

      //Assert
      string expectedKsql =
        @$"SELECT {nameof(Location.Longitude)}, {nameof(Location.Latitude)} FROM {streamName}
WHERE {nameof(Location.Latitude)} = '1' AND {nameof(Location.Longitude)} = 0.1 EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    #endregion

    #region Where

    [TestMethod]
    public void Where_BuildKSql_PrintsWhere()
    {
      //Arrange
      var query = CreateStreamSource()
        .Where(p => p.Latitude == "1");

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression);

      //Assert
      ksql.Should().BeEquivalentTo(@$"SELECT * FROM {streamName}
WHERE {nameof(Location.Latitude)} = '1' EMIT CHANGES;");
    }
    
    [TestMethod]
    public void WhereSelect_BuildKSql_PrintsSelectFromWhere()
    {
      //Arrange
      var query = CreateStreamSource()
        .Where(p => p.Latitude == "1")
        .Select(l => new {l.Longitude, l.Latitude});

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression);

      //Assert
      string expectedKsql =
        @$"SELECT {nameof(Location.Longitude)}, {nameof(Location.Latitude)} FROM {streamName}
WHERE {nameof(Location.Latitude)} = '1' EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    #endregion

    #region Take

    [TestMethod]
    public void Take_BuildKSql_PrintsLimit()
    {
      //Arrange
      int limit = 2;

      var query = CreateStreamSource()
        .Take(limit);

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression);

      //Assert
      ksql.Should().BeEquivalentTo(@$"SELECT * FROM {streamName} EMIT CHANGES LIMIT {limit};");
    }
    #endregion
  }
}