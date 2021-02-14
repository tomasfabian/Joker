using FluentAssertions;
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

    //[1,[4.2E-4,4.2E-4]]
  }
}