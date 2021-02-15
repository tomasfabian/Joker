﻿using FluentAssertions;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using UnitTests;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.RestApi.Parsers
{
  [TestClass]
  public class JsonArrayParserTests : TestBase
  {
    private JsonArrayParser ClassUnderTest { get; set; }

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      ClassUnderTest = new JsonArrayParser();
    }

    [TestMethod]
    public void CreateJson_PropertyNamesAreEqualToColumnHeaders()
    {
      //Arrange
      string[] headerColumns = { "KSQL_COL_0", "IsRobot" };
      string row = "{\"d\":4,\"c\":2},true";

      //Act
      var json = ClassUnderTest.CreateJson(headerColumns, row);

      //Assert
      json.Should().NotBeEmpty();

      var jObject = JObject.Parse(json);
      
      JProperty property = (JProperty)jObject.First;
      property.Name.Should().BeEquivalentTo(headerColumns[0]);
      
      property = (JProperty)jObject.Last;
      property.Name.Should().BeEquivalentTo(headerColumns[1]);
    }

    [TestMethod]
    public void CreateJson_PropertyValuesAreEqualToRowValues()
    {
      //Arrange
      string[] headerColumns = { "KSQL_COL_0", "IsRobot" };
      var mapValue = "{\"d\":4,\"c\":2}";
      string row = $"{mapValue}, true";

      //Act
      var json = ClassUnderTest.CreateJson(headerColumns, row);

      //Assert
      var jObject = JObject.Parse(json);

      var expectedJson = JObject.Parse(mapValue);
      JProperty property = (JProperty)jObject.First;
      property.Value.ToString().Should().BeEquivalentTo(expectedJson.ToString());

      property = (JProperty)jObject.Last;
      property.Value.ToString().Should().BeEquivalentTo("True");
    }

    [TestMethod]
    public void NestedArrayInMap()
    {
      //Arrange
      // {"queryId":"fd8ca155-a8e2-47f8-8cee-57fcd318a136","columnNames":["MAP"],"columnTypes":["MAP<STRING, ARRAY<INTEGER>>"]}
      // [{"a":[1,2],"b":[3,4]}]
      string[] headerColumns = { "MAP", "Value" };
      var mapValue = "{\"a\":[1,2],\"b\":[3,4]}";
      string row = $"{mapValue},1";

      //Act
      var json = ClassUnderTest.CreateJson(headerColumns, row);

      //Assert
      var jObject = JObject.Parse(json);

      var expectedJson = JObject.Parse(mapValue);
      JProperty property = (JProperty)jObject.First;
      property.Value.ToString().Should().BeEquivalentTo(expectedJson.ToString());

      property = (JProperty)jObject.Last;
      property.Value.ToString().Should().BeEquivalentTo("1");
    }

    [TestMethod]
    public void NestedArrayInArray()
    {
      //Arrange
      string[] headerColumns = { "Value", "Arr" };
      var mapValue = "[[1,2],[3,4]]";
      string row = $"1,{mapValue}";

      //Act
      var json = ClassUnderTest.CreateJson(headerColumns, row);

      //Assert
      json.Should().BeEquivalentTo("{\r\n\"Value\": 1\r\n,\"Arr\": [[1,2],[3,4]]\r\n}\r\n");
    }

    [TestMethod]
    public void NestedMapInArray()
    {
      //Arrange
      string[] headerColumns = { "Value", "Arr" };
      var mapValue = "[{\"a\":1,\"b\":2},{\"d\":4,\"c\":3}]";
      string row = $"1,{mapValue}";

      //Act
      var json = ClassUnderTest.CreateJson(headerColumns, row);

      //Assert
      json.Should().BeEquivalentTo("{\r\n\"Value\": 1\r\n,\"Arr\": [{\"a\":1,\"b\":2},{\"d\":4,\"c\":3}]\r\n}\r\n");
    }

    [TestMethod]
    public void NestedMapInMap()
    {
      //Arrange
      string[] headerColumns = { "Value", "Map" };
      var mapValue = "{\"a\":{\"a\":1,\"b\":2},\"b\":{\"d\":4,\"c\":3}}";
      string row = $"1,{mapValue}";

      //Act
      var json = ClassUnderTest.CreateJson(headerColumns, row);

      //Assert
      json.Should().BeEquivalentTo("{\r\n\"Value\": 1\r\n,\"Map\": {\"a\":{\"a\":1,\"b\":2},\"b\":{\"d\":4,\"c\":3}}\r\n}\r\n");
    }
  }
}