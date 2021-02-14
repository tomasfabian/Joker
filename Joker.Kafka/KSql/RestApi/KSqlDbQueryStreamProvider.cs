using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Kafka.DotNet.ksqlDB.Infrastructure.Extensions;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Exceptions;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Parsers;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Responses;

namespace Kafka.DotNet.ksqlDB.KSql.RestApi
{
  internal class KSqlDbQueryStreamProvider : KSqlDbProvider
  {
    private readonly IHttpClientFactory httpClientFactory;

    public KSqlDbQueryStreamProvider(IHttpClientFactory httpClientFactory)
      : base(httpClientFactory)
    {
      this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

#if NETCOREAPP3_1
      AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
#endif
    }

    public override string ContentType => "application/vnd.ksqlapi.delimited.v1";

    protected override string QueryEndPointName => "query-stream";

    protected override HttpClient OnCreateHttpClient()
    {
      var httpClient = base.OnCreateHttpClient();

#if !NETSTANDARD2_1
      httpClient.DefaultRequestVersion = new Version(2, 0);
#endif

      return httpClient;
    }

    private QueryStreamHeader queryStreamHeader;

    protected override RowValue<T> OnLineRed<T>(string rawJson)
    {
      //Console.WriteLine(rawJson);
      if (rawJson.Contains("queryId"))
      {
        queryStreamHeader = JsonSerializer.Deserialize<QueryStreamHeader>(rawJson);
      }      
      else if (rawJson.Contains("_error"))
      {
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(rawJson);

        if (errorResponse != null)
          throw new KSqlQueryException(errorResponse.Message)
          {
            ErrorCode = errorResponse.ErrorCode
          };
      }
      else
      {
        var result = rawJson.Substring(1, rawJson.Length - 2);
        
        var jsonSerializerOptions = GetOrCreateJsonSerializerOptions();

        if (queryStreamHeader.ColumnTypes.Length == 1 && !typeof(T).IsAnonymousType())
          return new RowValue<T>(JsonSerializer.Deserialize<T>(result, jsonSerializerOptions));

        var jsonRecord = new JsonArrayParser().CreateJson(queryStreamHeader.ColumnNames, result);

        var record = JsonSerializer.Deserialize<T>(jsonRecord, jsonSerializerOptions);

        return new RowValue<T>(record);
      }

      return default;
    }


    protected override HttpRequestMessage CreateQueryHttpRequestMessage(HttpClient httpClient, object parameters)
    {
      var httpRequestMessage = base.CreateQueryHttpRequestMessage(httpClient, parameters);

      httpRequestMessage.Version = HttpVersion.Version20;
#if NET5_0
      httpRequestMessage.VersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
#endif

      return httpRequestMessage;
    }
  }
}