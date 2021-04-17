﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Kafka.DotNet.ksqlDB.KSql.RestApi;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Statements;

namespace Kafka.DotNet.ksqlDB.Sample.Providers
{
  public class KSqlDbRestApiProvider : KSqlDbRestApiClient, IKSqlDbRestApiProvider
  {
    public static string KsqlDbUrl { get; } = @"http:\\localhost:8088";

    public static KSqlDbRestApiProvider Create(string ksqlDbUrl = null)
    {
      var uri = new Uri(ksqlDbUrl ?? KsqlDbUrl);

      return new KSqlDbRestApiProvider(new HttpClientFactory(uri));
    }

    public KSqlDbRestApiProvider(IHttpClientFactory httpClientFactory) 
      : base(httpClientFactory)
    {
    }

    public Task<HttpResponseMessage> DropStreamAndTopic(string streamName)
    {
      var statement = $"DROP STREAM IF EXISTS {streamName} DELETE TOPIC;";
      
      KSqlDbStatement ksqlDbStatement = new(statement);

      return ExecuteStatementAsync(ksqlDbStatement);
    }

    public Task<HttpResponseMessage> DropTableAndTopic(string tableName)
    {
      var statement = $"DROP TABLE IF EXISTS {tableName} DELETE TOPIC;";
      
      KSqlDbStatement ksqlDbStatement = new(statement);

      return ExecuteStatementAsync(ksqlDbStatement);
    }
  }
}