using FluentAssertions;
using Kafka.DotNet.ksqlDB.KSql.Linq;
using Kafka.DotNet.ksqlDB.KSql.Query;
using Kafka.DotNet.ksqlDB.KSql.Query.Context;
using Kafka.DotNet.ksqlDB.KSql.Query.Functions;
using Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;
using Location = Kafka.DotNet.ksqlDB.Tests.Models.Location;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Query
{
  [TestClass]
  public class KSqlQueryLanguageVisitorTests : TestBase
  {
    private KSqlQueryGenerator ClassUnderTest { get; set; }

    string streamName = nameof(Location) + "s";

    private KSqlDBContextOptions contextOptions;
    private QueryContext queryContext;

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      contextOptions = new KSqlDBContextOptions(TestParameters.KsqlDBUrl);
      queryContext = new QueryContext();
      ClassUnderTest = new KSqlQueryGenerator(contextOptions);
    }

    private IQbservable<Location> CreateStreamSource(bool shouldPluralizeStreamName = true)
    {
      contextOptions.ShouldPluralizeStreamName = shouldPluralizeStreamName;
      
      var context = new TestableDbProvider(contextOptions);
      
      return context.CreateQueryStream<Location>();
    }

    #region Select

    [TestMethod]
    public void SelectWhere_BuildKSql_PrintsSelectFromWhere()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(l => new { l.Longitude, l.Latitude })
        .Where(p => p.Latitude == "1");

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

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
        .Select(l => new { l.Longitude, l.Latitude });

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT {nameof(Location.Longitude)}, {nameof(Location.Latitude)} FROM {streamName}
WHERE {nameof(Location.Latitude)} = '1' AND {nameof(Location.Longitude)} = 0.1 EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }    
    
    [TestMethod]
    public void SelectDynamicFunction_BuildKSql_PrintsFunctionCall()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(c => new { c.Longitude, c.Latitude, Col = ksqlDB.KSql.Query.Functions.KSql.F.Dynamic("IFNULL(Latitude, 'n/a')") as string});

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT {nameof(Location.Longitude)}, {nameof(Location.Latitude)}, IFNULL(Latitude, 'n/a') Col FROM {streamName} EMIT CHANGES;";

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
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

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
        .Select(l => new { l.Longitude, l.Latitude });

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

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
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      ksql.Should().BeEquivalentTo(@$"SELECT * FROM {streamName} EMIT CHANGES LIMIT {limit};");
    }

    #endregion

    #region ToQueryString

    [TestMethod]
    public void ToQueryString_BuildKSql_PrintsQuery()
    {
      //Arrange
      int limit = 2;

      var query = CreateStreamSource()
        .Take(limit);

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      ksql.Should().BeEquivalentTo(@$"SELECT * FROM {streamName} EMIT CHANGES LIMIT {limit};");
    }

    #endregion

    #region StreamName

    [TestMethod]
    public void DontPluralize_BuildKSql_PrintsSingularStreamName()
    {
      //Arrange
      var query = CreateStreamSource(shouldPluralizeStreamName: false);

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT * FROM {nameof(Location)} EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    [TestMethod]
    public void InjectStreamName_BuildKSql_PrintsInjectedStreamName()
    {
      //Arrange
      queryContext.StreamName = "Custom_Stream_Name";
      var query = CreateStreamSource();

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT * FROM {queryContext.StreamName} EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    #endregion

    #region Arrays

    [TestMethod]
    public void SelectArrayLength_BuildKSql_PrintsArrayLength()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(c => new[] { 1, 2, 3 }.Length);

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT ARRAY_LENGTH(ARRAY[1, 2, 3]) FROM {streamName} EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    [TestMethod]
    public void SelectNamedArrayLength_BuildKSql_PrintsArrayLength()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(c => new { new[] { 1, 2, 3 }.Length } );

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT ARRAY_LENGTH(ARRAY[1, 2, 3]) Length FROM {streamName} EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    [TestMethod]
    public void SelectArrayIndex_BuildKSql_PrintsArrayIndex()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(c => new { FirstItem = new[] { 1, 2, 3 }[1] } );

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT ARRAY[1, 2, 3][1] AS FirstItem FROM {streamName} EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    #endregion
  }
}