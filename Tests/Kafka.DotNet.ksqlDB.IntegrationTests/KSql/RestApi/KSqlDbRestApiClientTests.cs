using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FluentAssertions;
using Kafka.DotNet.ksqlDB.KSql.RestApi;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Statements;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kafka.DotNet.ksqlDB.IntegrationTests.KSql.RestApi
{
  [TestClass]
  public class KSqlDbRestApiClientTests
  {
    [TestMethod]
    public async Task ExecuteStatementAsync()
    {
      //Arrange
      var ksqlDbUrl = @"http:\\localhost:8088";

      var httpClientFactory = new HttpClientFactory(new Uri(ksqlDbUrl));

      IKSqlDbRestApiClient restApiClient = new KSqlDbRestApiClient(httpClientFactory);

      string tableName = "TestTable";

      var statement = $@"CREATE OR REPLACE TABLE {tableName} (
        title VARCHAR PRIMARY KEY,
        id INT,
        release_year INT
      ) WITH (
        KAFKA_TOPIC='{tableName}',
        PARTITIONS=1,
        VALUE_FORMAT = 'JSON'
      );";

      KSqlDbStatement ksqlDbStatement = new(statement);

      //Act
      var httpResponseMessage = await restApiClient.ExecuteStatementAsync(ksqlDbStatement);

      //Assert
      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

      string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
      var responseObject = JsonSerializer.Deserialize<StatementResponse[]>(responseContent);

      responseObject?[0].CommandStatus.Status.Should().Be("SUCCESS");
      responseObject?[0].CommandStatus.Message.Should().Be("Table created");
    }
    
    public class StatementResponse
    {
      [JsonPropertyName("commandStatus")]
      public CommandStatus CommandStatus { get; set; }
    }

    public class CommandStatus
    {
      [JsonPropertyName("status")]
      public string Status { get; set; }

      [JsonPropertyName("message")]
      public string Message { get; set; }
    }
  }
}