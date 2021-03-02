using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Kafka.DotNet.ksqlDB.KSql.RestApi;

namespace Kafka.DotNet.ksqlDB.Sample.Providers
{
  public class KSqlDbRestApiProvider : IKSqlDbRestApiProvider
  {
    private readonly HttpClientFactory httpClientFactory;

    internal class KSqlStatement
    {
      public string ksql { get; set; }
    }

    public static string KsqlDbUrl { get; } = @"http:\\localhost:8088";

    public static KSqlDbRestApiProvider Create(string ksqlDbUrl = null)
    {
      var uri = new Uri(ksqlDbUrl ?? KsqlDbUrl);

      return new KSqlDbRestApiProvider(new HttpClientFactory(uri));
    }

    public KSqlDbRestApiProvider(HttpClientFactory httpClientFactory)
    {
      this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public async Task<HttpResponseMessage> ExecuteStatementAsync(string ksql)
    {
      try
      {
        using (var httpClient = httpClientFactory.CreateClient())
        {        
          var statement = new KSqlStatement
          {
            ksql = ksql
          };

          var json = JsonSerializer.Serialize(statement);

          var data = new StringContent(json, Encoding.UTF8, "application/json");

          httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/vnd.ksql.v1+json"));

          var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/ksql")
          {
            Content = data
          };

          var cancellationToken = new CancellationToken();

          var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

          return httpResponseMessage;
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw;
      }
    }

    public Task<HttpResponseMessage> DropStreamAndTopic(string streamName)
    {
      var statement = $"DROP STREAM IF EXISTS {streamName} DELETE TOPIC;";

      return ExecuteStatementAsync(statement);
    }

    public Task<HttpResponseMessage> DropTableAndTopic(string tableName)
    {
      var statement = $"DROP TABLE IF EXISTS {tableName} DELETE TOPIC;";

      return ExecuteStatementAsync(statement);
    }
  }
}