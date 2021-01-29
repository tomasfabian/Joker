using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Exceptions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Responses;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi
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

        if (queryStreamHeader.ColumnTypes.Length == 1)
          return new RowValue<T>(JsonSerializer.Deserialize<T>(result, jsonSerializerOptions));

        var jsonRecord = CreateJson(result);

        var record = JsonSerializer.Deserialize<T>(jsonRecord, jsonSerializerOptions);

        return new RowValue<T>(record);
      }

      return default;
    }

    private string CreateJson(string row)
    {
      var stringBuilder = new StringBuilder();

      stringBuilder.AppendLine("{");

      var headerColumns = queryStreamHeader.ColumnNames;
      bool isFirst = true;

      foreach (var column in headerColumns.Zip(row.Split(",").Select(c => c.Trim(' ')), (s, s1) => new { ColumnName = s, Value = s1 }))
      {
        if (!isFirst)
        {
          stringBuilder.Append(",");
        }

        stringBuilder.AppendLine($"\"{column.ColumnName}\": {column.Value}");

        isFirst = false;
      }

      stringBuilder.AppendLine("}");

      return stringBuilder.ToString();
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