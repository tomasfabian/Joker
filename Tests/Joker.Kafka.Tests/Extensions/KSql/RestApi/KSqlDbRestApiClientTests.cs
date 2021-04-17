using FluentAssertions;
using Kafka.DotNet.ksqlDB.KSql.RestApi;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Statements;
using Kafka.DotNet.ksqlDB.Tests.Fakes.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnitTests;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.RestApi
{
  [TestClass]
  public class KSqlDbRestApiClientTests : TestBase
  {
    private KSqlDbRestApiClient ClassUnderTest { get; set; }

    private IHttpClientFactory httpClientFactory;
    private HttpClient httpClient;

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      httpClientFactory = Mock.Of<IHttpClientFactory>();
      httpClient = FakeHttpClient.CreateWithResponse(StatementResponse);
      
      Mock.Get(httpClientFactory).Setup(c => c.CreateClient()).Returns(httpClient);

      ClassUnderTest = new KSqlDbRestApiClient(httpClientFactory);
    }
    
    string statement = "CREATE OR REPLACE TABLE movies";

    [TestMethod]
    public async Task ExecuteStatementAsync_HttpClientWasCalled_OkResult()
    {
      //Arrange
      var ksqlDbStatement = new KSqlDbStatement(statement);

      //Act
      var httpResponseMessage = await ClassUnderTest.ExecuteStatementAsync(ksqlDbStatement);

      //Assert
      httpResponseMessage.Should().NotBeNull();

      Mock.Get(httpClientFactory).Verify(c => c.CreateClient(), Times.Once);
    }
    
    [TestMethod]
    public void CreateHttpRequestMessage_HttpRequestMessage_WasConfigured()
    {
      //Arrange
      var ksqlDbStatement = new KSqlDbStatement(statement);

      //Act
      var httpRequestMessage = ClassUnderTest.CreateHttpRequestMessage(ksqlDbStatement);

      //Assert
      httpRequestMessage.Method.Should().Be(HttpMethod.Post);
      httpRequestMessage.RequestUri.Should().Be("/ksql");
      httpRequestMessage.Content.Headers.ContentType.MediaType.Should().Be(KSqlDbRestApiClient.MediaType);
    }
    
    [TestMethod]
    public async Task CreateHttpRequestMessage_HttpRequestMessage_ContentWasSet()
    {
      //Arrange
      var ksqlDbStatement = new KSqlDbStatement(statement);

      //Act
      var httpRequestMessage = ClassUnderTest.CreateHttpRequestMessage(ksqlDbStatement);

      //Assert
      var content = await httpRequestMessage.Content.ReadAsStringAsync();
      content.Should().Be(@$"{{""ksql"":""{statement}""}}");
    }

    [TestMethod]
    public void CreateContent_MediaTypeAndCharsetWereApplied()
    {
      //Arrange
      var ksqlDbStatement = new KSqlDbStatement(statement);

      //Act
      var stringContent = ClassUnderTest.CreateContent(ksqlDbStatement);

      //Assert
      stringContent.Headers.ContentType.MediaType.Should().Be(KSqlDbRestApiClient.MediaType);
      stringContent.Headers.ContentType.CharSet.Should().Be(Encoding.UTF8.WebName);
    }

    [TestMethod]
    public void CreateContent_Encoding_OverridenCharsetWasApplied()
    {
      //Arrange
      var encoding = Encoding.Unicode;

      var ksqlDbStatement = new KSqlDbStatement(statement)
      {
        ContentEncoding = encoding
      };

      //Act
      var stringContent = ClassUnderTest.CreateContent(ksqlDbStatement);

      //Assert
      stringContent.Headers.ContentType.CharSet.Should().Be(encoding.WebName);
    }

    [TestMethod]
    public async Task CreateContent_KSqlContentWasSet()
    {
      //Arrange
      var ksqlDbStatement = new KSqlDbStatement(statement);

      //Act
      var stringContent = ClassUnderTest.CreateContent(ksqlDbStatement);

      //Assert
      var content = await GetContent(stringContent);
      
      content.Should().Be(@$"{{""ksql"":""{statement}""}}");
    }

    [TestMethod]
    public async Task CreateContent_CommandSequenceNumber()
    {
      //Arrange
      long commandSequenceNumber = 1000;
      var ksqlDbStatement = new KSqlDbStatement(statement)
      {
        CommandSequenceNumber = commandSequenceNumber
      };

      //Act
      var stringContent = ClassUnderTest.CreateContent(ksqlDbStatement);

      //Assert
      var content = await GetContent(stringContent);

      content.Should().Be(@$"{{""ksql"":""{statement}"",""commandSequenceNumber"":{commandSequenceNumber}}}");
    }

    private static async Task<string> GetContent(StringContent stringContent)
    {
      var byteArray = await stringContent.ReadAsByteArrayAsync();

      var content = Encoding.Default.GetString(byteArray);

      return content;
    }

    [TestMethod]
    public void GetEndpoint_DefaultIs_KSql()
    {
      //Arrange
      var ksqlDbStatement = new KSqlDbStatement(statement);

      //Act
      var endpoint = KSqlDbRestApiClient.GetEndpoint(ksqlDbStatement);

      //Assert
      endpoint.Should().Be("/ksql");
    }

    [TestMethod]
    public void GetEndpoint_OverridenToQueryEndpoint()
    {
      //Arrange
      var ksqlDbStatement = new KSqlDbStatement(statement)
      {
        EndpointType = EndpointType.Query
      };

      //Act
      var endpoint = KSqlDbRestApiClient.GetEndpoint(ksqlDbStatement);

      //Assert
      endpoint.Should().Be("/query");
    }

    private string StatementResponse = @"[{""@type"":""currentStatus"",""statementText"":""CREATE OR REPLACE TABLE MOVIES (TITLE STRING PRIMARY KEY, ID INTEGER, RELEASE_YEAR INTEGER) WITH (KAFKA_TOPIC='Movies', KEY_FORMAT='KAFKA', PARTITIONS=1, VALUE_FORMAT='JSON');"",""commandId"":""table/`MOVIES`/create"",""commandStatus"":{""status"":""SUCCESS"",""message"":""Table created"",""queryId"":null},""commandSequenceNumber"":328,""warnings"":[]}]
";
  }
}