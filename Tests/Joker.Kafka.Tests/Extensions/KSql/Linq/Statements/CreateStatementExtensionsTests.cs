using FluentAssertions;
using Kafka.DotNet.ksqlDB.KSql.Linq.Statements;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Serialization;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Statements;
using Kafka.DotNet.ksqlDB.Tests.Helpers;
using Kafka.DotNet.ksqlDB.Tests.Models;
using Kafka.DotNet.ksqlDB.Tests.Pocos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Kafka.DotNet.ksqlDB.KSql.RestApi;
using Kafka.DotNet.ksqlDB.Tests.Fakes.Http;
using Moq;
using UnitTests;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Linq.Statements
{
  [TestClass]
  public class CreateStatementExtensionsTests : TestBase
    {
      private TestableDbProvider DbProvider { get; set; }

      protected virtual string StatementResponse { get; set; } = @"[{""@type"":""currentStatus"", ""commandSequenceNumber"":2174,""warnings"":[]}]";

      [TestInitialize]
      public override void TestInitialize()
      {
        base.TestInitialize();

        var httpClientFactory = Mock.Of<IHttpClientFactory>();
        var httpClient = FakeHttpClient.CreateWithResponse(StatementResponse);

        Mock.Get(httpClientFactory).Setup(c => c.CreateClient()).Returns(() => httpClient);

        DbProvider = new TestableDbProvider(TestParameters.KsqlDBUrl, httpClientFactory);
      }

      private const string StreamName = "TestStream";

      [TestMethod]
      public void CreateOrReplaceStreamStatement_ToStatementString_CalledTwiceWithSameResult()
      {
        //Arrange
        var query = DbProvider.CreateOrReplaceStreamStatement(StreamName)
          .As<Location>();

        //Act
        var ksql1 = query.ToStatementString();
        var ksql2 = query.ToStatementString();

        //Assert
        ksql1.Should().BeEquivalentTo(@$"CREATE OR REPLACE STREAM {StreamName}
AS SELECT * FROM {nameof(Location)}s EMIT CHANGES;");

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

        var query = DbProvider.CreateOrReplaceStreamStatement(StreamName)
          .With(creationMetadata)
          .As<Movie>()
          .Where(c => c.Id < 3)
          .Select(c => new {c.Title, ReleaseYear = c.Release_Year})
          .PartitionBy(c => c.Title);

        //Act
        var ksql = query.ToStatementString();

        //Assert
        ksql.Should().BeEquivalentTo(@$"CREATE OR REPLACE STREAM {StreamName}
 WITH ( KAFKA_TOPIC='tweetsByTitle', KEY_FORMAT='Json', VALUE_FORMAT='Json', PARTITIONS='1', REPLICAS='1' )
AS SELECT Title, Release_Year AS ReleaseYear FROM Movies
WHERE Id < 3 PARTITION BY Title EMIT CHANGES;");
      }

      private const string TableName = "TestTable";

      [TestMethod]
      public async Task CreateOrReplaceTableStatement_ExecuteStatementAsync_ResponseWasReceived()
      {
        //Arrange
        var query = DbProvider.CreateOrReplaceTableStatement(TableName)
          .As<Location>();

        //Act
        var httpResponseMessage = await query.ExecuteStatementAsync();

        //Assert
        string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
        responseContent.Should().BeEquivalentTo(StatementResponse);
      }
    }
}