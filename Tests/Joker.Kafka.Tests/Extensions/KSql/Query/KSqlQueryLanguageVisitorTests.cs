using System.Linq;
using FluentAssertions;
using Joker.Kafka.Extensions.KSql.Query;
using Joker.Kafka.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;
using static System.String;

namespace Joker.Kafka.Tests.Extensions.KSql.Query
{
  [TestClass]
  public class KSqlQueryLanguageVisitorTests : TestBase<KSqlQueryLanguageVisitor>
  {
    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      ClassUnderTest = new KSqlQueryLanguageVisitor();
    }

    #region Select

    [TestMethod]
    public void Select_BuildKSql_PrintsSelect()
    {
      //Arrange
      var query = new[]
        {
          new Location {Latitude = "1"},
          new Location {Latitude = "2"}
        }.AsQueryable()
        .Select(l => new {l.Longitude, l.Latitude});

      //TODO: stream name

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression);

      //Assert
      ksql.Should().BeEquivalentTo($"SELECT {nameof(Location.Longitude)}, {nameof(Location.Latitude)} FROM ");
    }
    
    [TestMethod]
    public void SelectWhere_BuildKSql_PrintsSelectFromWhere()
    {
      //Arrange
      var query = new[]
        {
          new Location {Latitude = "1"},
          new Location {Latitude = "2"}
        }.AsQueryable()
        .Select(l => new {l.Longitude, l.Latitude})
        .Where(p => p.Latitude == "1");

      string streamName = Empty; //TODO: stream name

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression);

      //Assert
      string expectedKsql =
        $"SELECT {nameof(Location.Longitude)}, {nameof(Location.Latitude)} FROM {streamName}WHERE {nameof(Location.Latitude)} = '1'";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    [TestMethod]
    public void SelectMultipleWhere_BuildKSql_PrintsSelectFromWheres()
    {
      //Arrange
      var query = new[]
        {
          new Location {Latitude = "1"},
          new Location {Latitude = "2"}
        }.AsQueryable()
        .Where(p => p.Latitude == "1")
        .Where(p => p.Longitude == 0.1)
        .Select(l => new {l.Longitude, l.Latitude});

      string streamName = Empty; //TODO: stream name

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression);

      //Assert
      string expectedKsql =
        $"SELECT {nameof(Location.Longitude)}, {nameof(Location.Latitude)} FROM {streamName}WHERE {nameof(Location.Latitude)} = '1' AND {nameof(Location.Longitude)} = 0.1";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    #endregion

    #region Where

    [TestMethod]
    public void Where_BuildKSql_PrintsWhere()
    {
      //Arrange
      var query = new[]
        {
          new Location { Latitude = "1" },
          new Location { Latitude = "2" }
        }.AsQueryable()
        .Where(p => p.Latitude == "1");
      
      string streamName = Empty; //TODO: stream name

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression);

      //Assert
      ksql.Should().BeEquivalentTo($"SELECT * FROM {streamName}WHERE {nameof(Location.Latitude)} = '1'");
    }
    
    [TestMethod]
    public void WhereSelect_BuildKSql_PrintsSelectFromWhere()
    {
      //Arrange
      var query = new[]
        {
          new Location {Latitude = "1"},
          new Location {Latitude = "2"}
        }.AsQueryable()
        .Where(p => p.Latitude == "1")
        .Select(l => new {l.Longitude, l.Latitude});

      string streamName = Empty; //TODO: stream name

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression);

      //Assert
      string expectedKsql =
        $"SELECT {nameof(Location.Longitude)}, {nameof(Location.Latitude)} FROM {streamName}WHERE {nameof(Location.Latitude)} = '1'";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    #endregion
  }
}