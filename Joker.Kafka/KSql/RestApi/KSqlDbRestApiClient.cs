using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Kafka.DotNet.ksqlDB.KSql.Query.Context;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Enums;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Statements;

namespace Kafka.DotNet.ksqlDB.KSql.RestApi
{
  public class KSqlDbRestApiClient : IKSqlDbRestApiClient
  {
    private readonly IHttpClientFactory httpClientFactory;

    public KSqlDbRestApiClient(IHttpClientFactory httpClientFactory)
    {
      this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    internal static readonly string MediaType = "application/vnd.ksql.v1+json";

    public async Task<HttpResponseMessage> ExecuteStatementAsync(KSqlDbStatement ksqlDbStatement, CancellationToken cancellationToken = default)
    {
      using var httpClient = httpClientFactory.CreateClient();

      var httpRequestMessage = CreateHttpRequestMessage(ksqlDbStatement);

      httpClient.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue(MediaType));

      var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, cancellationToken)
        .ConfigureAwait(false);

      return httpResponseMessage;
    }

    internal HttpRequestMessage CreateHttpRequestMessage(KSqlDbStatement ksqlDbStatement)
    {
      var data = CreateContent(ksqlDbStatement);

      var endpoint = GetEndpoint(ksqlDbStatement);

      var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint)
      {
        Content = data
      };

      return httpRequestMessage;
    }

    internal StringContent CreateContent(KSqlDbStatement ksqlDbStatement)
    {
      var json = JsonSerializer.Serialize(ksqlDbStatement);

      var data = new StringContent(json, ksqlDbStatement.ContentEncoding, MediaType);

      return data;
    }

    internal static string GetEndpoint(KSqlDbStatement ksqlDbStatement)
    {
      var endpoint = ksqlDbStatement.EndpointType switch
      {
        EndpointType.KSql => "/ksql",
        EndpointType.Query => "/query",
        _ => throw new ArgumentOutOfRangeException()
      };

      return endpoint;
    }

    #region Creation
    
    public Task<HttpResponseMessage> CreateStream<T>(EntityCreationMetadata creationMetadata, bool ifNotExists = false, CancellationToken cancellationToken = default)
    {
      var statementContext = new StatementContext
      {
        CreationType = CreationType.Create,
        KSqlEntityType = KSqlEntityType.Stream
      };

      return CreateOrReplace<T>(statementContext, creationMetadata, ifNotExists, cancellationToken);
    }

    public Task<HttpResponseMessage> CreateOrReplaceStream<T>(EntityCreationMetadata creationMetadata, CancellationToken cancellationToken = default)
    {
      var statementContext = new StatementContext
      {
        CreationType = CreationType.CreateOrReplace,
        KSqlEntityType = KSqlEntityType.Stream
      };

      return CreateOrReplace<T>(statementContext, creationMetadata, ifNotExists: null, cancellationToken);
    }
    
    public Task<HttpResponseMessage> CreateTable<T>(EntityCreationMetadata creationMetadata, bool ifNotExists = false, CancellationToken cancellationToken = default)
    {
      var statementContext = new StatementContext
      {
        CreationType = CreationType.Create,
        KSqlEntityType = KSqlEntityType.Table
      };

      return CreateOrReplace<T>(statementContext, creationMetadata, ifNotExists, cancellationToken);
    }

    public Task<HttpResponseMessage> CreateOrReplaceTable<T>(EntityCreationMetadata creationMetadata, CancellationToken cancellationToken = default)
    {
      var statementContext = new StatementContext
      {
        CreationType = CreationType.CreateOrReplace,
        KSqlEntityType = KSqlEntityType.Table
      };

      return CreateOrReplace<T>(statementContext, creationMetadata, ifNotExists: null, cancellationToken);
    }

    private Task<HttpResponseMessage> CreateOrReplace<T>(StatementContext statementContext, EntityCreationMetadata creationMetadata, bool? ifNotExists, CancellationToken cancellationToken = default)
    {
      string ksql = new CreateEntity().Print<T>(statementContext, creationMetadata, ifNotExists);

      var ksqlStatement = new KSqlDbStatement(ksql);

      return ExecuteStatementAsync(ksqlStatement, cancellationToken);
    }

    #endregion
  }
}