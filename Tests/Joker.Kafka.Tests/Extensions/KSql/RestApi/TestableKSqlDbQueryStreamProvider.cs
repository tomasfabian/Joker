using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi;
using Moq;
using Moq.Protected;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.RestApi
{
  public class TestableKSqlDbQueryStreamProvider : KSqlDbQueryStreamProvider<KSqlDbProviderTests.Tweet>
  {
    public TestableKSqlDbQueryStreamProvider(IHttpClientFactory httpClientFactory) 
      : base(httpClientFactory)
    {
    }

    public bool ShouldThrowException { get; set; }

    protected override HttpClient OnCreateHttpClient()
    {      
      var queryResponse =
        @"{""queryId"":""59df818e-7d88-436f-95ac-3c59becc9bfb"",""columnNames"":[""MESSAGE"",""ID"",""ISROBOT"",""AMOUNT""],""columnTypes"":[""STRING"",""INTEGER"",""BOOLEAN"",""DOUBLE""]}
[""Hello world"",1,true,0.1]
[""Wall-e"",2,false,1.1]";

      var errorResponse =
        @"{""@type"":""generic_error"",""error_code"":40001,""message"":""Line: 1, Col: 21: SELECT column 'KVAK' cannot be resolved.\nStatement: SELECT Message, Id, Kvak FROM Tweets\r\nWHERE Message = 'Hello world' EMIT CHANGES LIMIT 2;""}";

      var handlerMock = new Mock<HttpMessageHandler>();

      handlerMock
        .Protected()
        .Setup<Task<HttpResponseMessage>>(
          nameof(HttpClient.SendAsync),
          ItExpr.IsAny<HttpRequestMessage>(),
          ItExpr.IsAny<CancellationToken>()
        )
        .ReturnsAsync(new HttpResponseMessage()
        {
          StatusCode = HttpStatusCode.OK,
          Content = new StringContent(ShouldThrowException ? errorResponse : queryResponse),
        })
        .Verifiable();

      return new HttpClient(handlerMock.Object)
      {
        BaseAddress = new Uri(@"http:\\localhost:8088")
      };
    }
  }
}