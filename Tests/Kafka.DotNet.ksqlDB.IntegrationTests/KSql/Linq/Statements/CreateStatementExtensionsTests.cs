﻿using System.Threading.Tasks;
using FluentAssertions;
using Kafka.DotNet.ksqlDB.IntegrationTests.Models.Movies;
using Kafka.DotNet.ksqlDB.KSql.Linq.Statements;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Extensions;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Serialization;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Statements;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kafka.DotNet.ksqlDB.IntegrationTests.KSql.Linq.Statements
{
  [TestClass]
  public class CreateStatementExtensionsTests : IntegrationTests
  {
    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();
    }

    private const string StreamName = "TestStream";

    [TestMethod]
    public void CreateOrReplaceStreamStatement_ToStatementString_CalledTwiceWithSameResult()
    {
      //Arrange
      var query = Context.CreateOrReplaceStreamStatement(StreamName)
        .As<Movie>();

      //Act
      var ksql1 = query.ToStatementString();
      var ksql2 = query.ToStatementString();

      //Assert
      ksql1.Should().BeEquivalentTo(@$"CREATE OR REPLACE STREAM {StreamName}
AS SELECT * FROM {nameof(Movie)}s EMIT CHANGES;");

      ksql1.Should().BeEquivalentTo(ksql2);
    }

    [TestMethod]
    public void CreateOrReplaceStreamStatement_ToStatementString_ComplexQueryWasGenerated()
    {
      //Arrange
      var creationMetadata = new CreationMetadata
      {
        KafkaTopic = "tweetsByTitle",
        KeyFormat = SerializationFormats.Json,
        ValueFormat = SerializationFormats.Json,
        Replicas = 1,
        Partitions = 1
      };

      var query = Context.CreateOrReplaceStreamStatement(StreamName)
        .With(creationMetadata)
        .As<Movie>()
        .Where(c => c.Id < 3)
        .Select(c => new { c.Title, ReleaseYear = c.Release_Year })
        .PartitionBy(c => c.Title);

      //Act
      var ksql = query.ToStatementString();

      //Assert
      ksql.Should().BeEquivalentTo(@$"CREATE OR REPLACE STREAM {StreamName}
 WITH ( KAFKA_TOPIC='tweetsByTitle', KEY_FORMAT='Json', VALUE_FORMAT='Json', PARTITIONS='1', REPLICAS='1' )
AS SELECT Title, Release_Year AS ReleaseYear FROM Movies
WHERE Id < 3 PARTITION BY Title EMIT CHANGES;");
    }

    private const string TableName = "IntegrationTestTable";

    [TestMethod]
    public async Task CreateOrReplaceTableStatement_ExecuteStatementAsync_ResponseWasReceived()
    {
      //Arrange
      var query = Context.CreateOrReplaceTableStatement(TableName)
        .As<Movie>();

      //Act
      var httpResponseMessage = await query.ExecuteStatementAsync();

      //Assert
      string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
      responseContent.Should().NotBeNull();

      var response = httpResponseMessage.ToStatementResponses()[0];
      response.CommandStatus.Status.Should().Be("SUCCESS");
    }
    //[{"@type":"currentStatus","statementText":"CREATE OR REPLACE TABLE INTEGRATIONTESTTABLE WITH (KAFKA_TOPIC='INTEGRATIONTESTTABLE', PARTITIONS=1, REPLICAS=1) AS SELECT *\nFROM MOVIES MOVIES\nEMIT CHANGES;","commandId":"table/`INTEGRATIONTESTTABLE`/create","commandStatus":{"status":"SUCCESS","message":"Created query with ID CTAS_INTEGRATIONTESTTABLE_2257","queryId":"CTAS_INTEGRATIONTESTTABLE_2257"},"commandSequenceNumber":2258,"warnings":[]}]
  }
}