﻿using System.Threading.Tasks;
using FluentAssertions;
using Kafka.DotNet.ksqlDB.KSql.Linq.PullQueries;
using Kafka.DotNet.ksqlDB.KSql.Query.Functions;
using Kafka.DotNet.ksqlDB.KSql.RestApi;
using Kafka.DotNet.ksqlDB.Tests.Fakes.Http;
using Kafka.DotNet.ksqlDB.Tests.Helpers;
using Kafka.DotNet.ksqlDB.Tests.Models.Sensors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnitTests;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Linq.PullQueries
{
  [TestClass]
  public class PullQueryExtensionsTests : TestBase
  {
    private TestableDbProvider DbProvider { get; set; }

    protected virtual string PullQueryResponse { get; set; } = @"[{""header"":{""queryId"":""query_1619945627639"",""schema"":""`SENSORID` STRING KEY, `WINDOWSTART` BIGINT KEY, `WINDOWEND` BIGINT KEY, `AVGVALUE` DOUBLE""}},
{""row"":{""columns"":[""sensor-1"",1619859745000,1619859750000,11.0]}},";

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      var httpClientFactory = Mock.Of<IHttpClientFactory>();
      var httpClient = FakeHttpClient.CreateWithResponse(PullQueryResponse);

      Mock.Get(httpClientFactory).Setup(c => c.CreateClient()).Returns(() => httpClient);

      DbProvider = new TestableDbProvider(TestParameters.KsqlDBUrl, httpClientFactory);
    }

    [TestMethod]
    public async Task CreatePullQuery()
    {
      //Arrange
      string sensorId = "sensor-1";

      //Act
      var result = await DbProvider.CreatePullQuery<IoTSensorStats>()
        .Where(c => c.SensorId == sensorId)
        .GetAsync();
      
      //Assert
      result.Should().NotBeNull();
      result.SensorId.Should().Be(sensorId);
    }

    [TestMethod]
    public void CreatePullQuery_ToQueryString()
    {
      //Arrange
      string sensorId = "sensor-1";      
      var query = DbProvider.CreatePullQuery<IoTSensorStats>()
        .Where(c => c.SensorId == sensorId);

      //Act
      var ksql1 = query.ToQueryString();
      var ksql2 = query.ToQueryString();

      //Assert
      ksql1.Should().BeEquivalentTo(@$"SELECT * FROM {nameof(IoTSensorStats)}
WHERE SensorId = '{sensorId}';");

      ksql1.Should().BeEquivalentTo(ksql2);
    }

    private const string MaterializedViewName = "TestView";

    [TestMethod]
    public void CreatePullQuery_TableNameWasOverriden()
    {
      //Arrange
      string sensorId = "sensor-1";      
      var query = DbProvider.CreatePullQuery<IoTSensorStats>(MaterializedViewName)
        .Where(c => c.SensorId == sensorId);

      //Act
      var ksql = query.ToQueryString();

      //Assert
      ksql.Should().BeEquivalentTo(@$"SELECT * FROM {MaterializedViewName}
WHERE SensorId = '{sensorId}';");
    }

    [TestMethod]
    public void CreatePullQuery_WindowBounds_LongTypeDateFormat()
    {
      //Arrange
      string sensorId = "sensor-1";      
      var query = DbProvider.CreatePullQuery<IoTSensorStats>(MaterializedViewName)
        .Where(c => c.SensorId == sensorId);

      long windowStart = 1575044700000;
      long windowEnd = 1675044700000;

      //Act
      var ksql = query.Where(c => Bounds.WindowStart > windowStart && Bounds.WindowEnd <= windowEnd).ToQueryString();

      //Assert
      ksql.Should().BeEquivalentTo(@$"SELECT * FROM {MaterializedViewName}
WHERE SensorId = '{sensorId}' AND (WINDOWSTART > {windowStart}) AND (WINDOWEND <= {windowEnd});");
    }

    [TestMethod]
    public void CreatePullQuery_WindowBounds_StringTypeDateFormat()
    {
      //Arrange
      string sensorId = "sensor-1";      
      var query = DbProvider.CreatePullQuery<IoTSensorStats>(MaterializedViewName)
        .Where(c => c.SensorId == sensorId);

      string windowStart = "2019-10-03T21:31:16";
      string windowEnd = "2020-10-03T21:31:16";

      //Act
      var ksql = query.Where(c => Bounds.WindowStart > windowStart && Bounds.WindowEnd <= windowEnd).ToQueryString();

      //Assert
      ksql.Should().BeEquivalentTo(@$"SELECT * FROM {MaterializedViewName}
WHERE SensorId = '{sensorId}' AND (WINDOWSTART > '{windowStart}') AND (WINDOWEND <= '{windowEnd}');");
    }
  }
}