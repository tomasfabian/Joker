﻿using System.Collections.Generic;
using FluentAssertions;
using Kafka.DotNet.ksqlDB.KSql.Linq;
using Kafka.DotNet.ksqlDB.KSql.Query;
using Kafka.DotNet.ksqlDB.KSql.Query.Context;
using Kafka.DotNet.ksqlDB.KSql.Query.Functions;
using Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Tests.Helpers;
using Kafka.DotNet.ksqlDB.Tests.Pocos;
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

    private IQbservable<Tweet> CreateTweetsStreamSource()
    {
      var context = new TestableDbProvider(contextOptions);

      return context.CreateQueryStream<Tweet>();
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
        .Select(c => new { c.Longitude, c.Latitude, Col = ksqlDB.KSql.Query.Functions.KSql.F.Dynamic("IFNULL(Latitude, 'n/a')") as string });

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
        .Select(c => new { new[] { 1, 2, 3 }.Length });

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
        .Select(c => new { FirstItem = new[] { 1, 2, 3 }[1] });

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT ARRAY[1, 2, 3][1] AS FirstItem FROM {streamName} EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    [TestMethod]
    public void ArrayProjected()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(c => new { Str = new[] { 1, 2, 3 } });

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT ARRAY[1, 2, 3] Str FROM {streamName} EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    #endregion

    #region Maps

    [TestMethod]
    public void SelectDictionary_BuildKSql_PrintsMap()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(c => new Dictionary<string, int>
        {
          { "c", 2 },
          { "d", 4 }
        });

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT MAP('c' := 2, 'd' := 4) FROM {streamName} EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    [TestMethod]
    public void SelectDictionaryProjected_BuildKSql_PrintsMap()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(c => new
        {
          Map = new Dictionary<string, int>
          {
            { "c", 2 },
            { "d", 4 }
          }
        });

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT MAP('c' := 2, 'd' := 4) Map FROM {streamName} EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    [TestMethod]
    public void SelectDictionaryElement_BuildKSql_PrintsMapElementAccess()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(c => new Dictionary<string, int>
        {
          { "c", 2 },
          { "d", 4 }
        }["d"]);

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT MAP('c' := 2, 'd' := 4)['d'] FROM {streamName} EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    [TestMethod]
    public void SelectDictionaryElementProjected_BuildKSql_PrintsMapElementAccess()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(c => new
        {
          Element = new Dictionary<string, int>
          {
            { "c", 2 },
            { "d", 4 }
          }["d"]
        });

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT MAP('c' := 2, 'd' := 4)['d'] Element FROM {streamName} EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    #endregion

    #region Structs

    private struct Point
    {
      public int X { get; set; }

      public int Y { get; set; }
    }

    [TestMethod]
    public void SelectStruct_BuildKSql_PrintsStruct()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(c => new Point {X = 1, Y = 2});

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT STRUCT(X := 1, Y := 2) FROM {streamName} EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    [TestMethod]
    public void SelectStructProjected_BuildKSql_PrintsStruct()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(c => new { V = new Point {X = 1, Y = 2} });

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT STRUCT(X := 1, Y := 2) V FROM {streamName} EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    [TestMethod]
    public void SelectStructElement_BuildKSql_PrintsElementAccessor()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(c => new Point {X = 1, Y = 2}.X);

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT STRUCT(X := 1, Y := 2)->X FROM {streamName} EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    [TestMethod]
    public void SelectStructElementProjected_BuildKSql_PrintsElementAccessor()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(c => new { X = new Point {X = 1, Y = 2}.X });

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT STRUCT(X := 1, Y := 2)->X X FROM {streamName} EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    private struct LocationStruct
    {
      public string X { get; set; }

      public double Y { get; set; }
    }

    [TestMethod]
    public void SelectStructElementsFromColumns_BuildKSql_PrintsStruct()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(c => new { LS = new LocationStruct {X = c.Latitude, Y = c.Longitude}, Text = "text" });

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT STRUCT(X := {nameof(Location.Latitude)}, Y := {nameof(Location.Longitude)}) LS, 'Text' Text FROM {streamName} EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    #endregion

    #region Deeply nested types

    [TestMethod]
    public void NestedArrayInMap()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(c => new
        {
          Map = new Dictionary<string, int[]>
          {
            { "a", new[] { 1, 2 } },
            { "b", new[] { 3, 4 } },
          }
        });

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT MAP('a' := ARRAY[1, 2], 'b' := ARRAY[3, 4]) Map FROM {streamName} EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    [TestMethod]
    public void NestedMapInMap()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(c => new
        {
          Map = new Dictionary<string, Dictionary<string, int>>
          {
            { "a", new Dictionary<string, int> { { "a", 1 }, { "b", 2 } } },
            { "b", new Dictionary<string, int> { { "c", 3 }, { "d", 4 } } },
          }
        });

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT MAP('a' := MAP('a' := 1, 'b' := 2), 'b' := MAP('c' := 3, 'd' := 4)) Map FROM {streamName} EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    [TestMethod]
    public void NestedMapInArray()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(c => new
        {
          Arr = new[]
          {
            new Dictionary<string, int> { { "a", 1 }, { "b", 2 } },
            new Dictionary<string, int> { { "c", 3 }, { "d", 4 } }
          }
        });

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT ARRAY[MAP('a' := 1, 'b' := 2), MAP('c' := 3, 'd' := 4)] Arr FROM {streamName} EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    [TestMethod]
    public void NestedArrayInArray()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(c => new
        {
          Arr = new[]
          {
            new [] { 1, 2},
            new [] { 3, 4},
          }
        });

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT ARRAY[ARRAY[1, 2], ARRAY[3, 4]] Arr FROM {streamName} EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    [TestMethod]
    [Ignore("TODO")]
    public void NestedArrayInArray_OuterMemberAccess()
    {
      //Arrange
      var nestedArrays = new[]
      {
        new[] {1, 2},
        new[] {3, 4},
      };

      var query = CreateStreamSource()
        .Select(c => new
        {
          Arr = nestedArrays
        });

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT ARRAY[ARRAY[1, 2], ARRAY[3, 4]] Arr FROM {streamName} EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    #endregion

    #region Deeply nested types element destructure

    [TestMethod]
    public void NestedArrayInMap_ElementAccess()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(c => new
        {
          Map = new Dictionary<string, int[]>
          {
            { "a", new[] { 1, 2 } },
            { "b", new[] { 3, 4 } },
          }["a"][1]
        });

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT MAP('a' := ARRAY[1, 2], 'b' := ARRAY[3, 4])['a'][1] AS Map FROM {streamName} EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    [TestMethod]
    public void NestedMapInMap_ElementAccess()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(c => new
        {
          Map = new Dictionary<string, Dictionary<string, int>>
          {
            { "a", new Dictionary<string, int> { { "a", 1 }, { "b", 2 } } },
            { "b", new Dictionary<string, int> { { "c", 3 }, { "d", 4 } } },
          }["a"]["d"]
        });

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT MAP('a' := MAP('a' := 1, 'b' := 2), 'b' := MAP('c' := 3, 'd' := 4))['a']['d'] Map FROM {streamName} EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    [TestMethod]
    public void NestedMapInArray_ElementAccess()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(c => new
        {
          Arr = new[]
          {
            new Dictionary<string, int> { { "a", 1 }, { "b", 2 } },
            new Dictionary<string, int> { { "c", 3 }, { "d", 4 } }
          }[1]["d"]
        });

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT ARRAY[MAP('a' := 1, 'b' := 2), MAP('c' := 3, 'd' := 4)][1]['d'] Arr FROM {streamName} EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }
    
    [TestMethod]
    public void NestedArrayInArray_ElementAccess()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(c => new
        {
          Arr = new[]
          {
            new [] { 1, 2},
            new [] { 3, 4},
          }[0][1]
        });

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT ARRAY[ARRAY[1, 2], ARRAY[3, 4]][0][1] AS Arr FROM {streamName} EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    #endregion

    #region Operators

    [TestMethod]
    public void LogicalOperatorNot_BuildKSql_PrintsNot()
    {
      //Arrange
      var query = CreateTweetsStreamSource()
        .Select(l => !l.IsRobot);

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT NOT {nameof(Tweet.IsRobot)} FROM {nameof(Tweet)}s EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    [TestMethod]
    public void LogicalOperatorNotProjected_BuildKSql_PrintsNot()
    {
      //Arrange
      var query = CreateTweetsStreamSource()
        .Select(l => new { NotRobot = !l.IsRobot });

      //Act
      var ksql = ClassUnderTest.BuildKSql(query.Expression, queryContext);

      //Assert
      string expectedKsql =
        @$"SELECT NOT {nameof(Tweet.IsRobot)} NotRobot FROM {nameof(Tweet)}s EMIT CHANGES;";

      ksql.Should().BeEquivalentTo(expectedKsql);
    }

    #endregion
  }
}