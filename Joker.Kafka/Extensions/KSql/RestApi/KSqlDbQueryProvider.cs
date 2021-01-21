using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Exceptions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Responses;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi
{
  public class KSqlDbQueryProvider<T> : KSqlDbProvider<T>
  {
    public KSqlDbQueryProvider(IHttpClientFactory httpClientFactory) 
      : base(httpClientFactory)
    {
    }

    public override string ContentType => "application/vnd.ksql.v1+json";

    protected override string QueryEndPointName => "query";

    private HeaderResponse headerResponse;

    protected override T OnLineRed(string rawJson)
    {
      if (rawJson.StartsWith("["))
        rawJson = rawJson.Substring(startIndex: 1);
      if (rawJson.EndsWith(","))
        rawJson = rawJson.Substring(0, rawJson.Length - 1);
      if (rawJson.EndsWith("]"))
        rawJson = rawJson.Substring(0, rawJson.Length - 1);

      if (rawJson.Contains("statement_error"))
      {
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(rawJson);

        if (errorResponse != null)
          throw new KSqlQueryException(errorResponse.Message)
          {
            Statement = errorResponse.StatementText,
            ErrorCode = errorResponse.ErrorCode
          };
      }

      if (rawJson.Contains("header"))
        headerResponse = JsonSerializer.Deserialize<HeaderResponse>(rawJson);

      if (rawJson.Contains("row"))
      {
        var result = JsonSerializer.Deserialize<RowResponse>(rawJson);

        var jsonRecord = CreateJson(result);
        var jsonSerializerOptions = GetOrCreateJsonSerializerOptions();
        var record = JsonSerializer.Deserialize<T>(jsonRecord, jsonSerializerOptions);

        return record;
      }

      return default;
    }

    private string CreateJson(RowResponse rowResponse)
    {
      var stringBuilder = new StringBuilder();

      stringBuilder.AppendLine("{");

      var headerColumns = headerResponse.Header.Schema.Split(",");
      var rowColumns = rowResponse.Row.Columns;

      bool isFirst = true;

      foreach (var column in headerColumns.Zip(rowColumns, (s, s1) => new { ColumnName = s, Value = s1 }))
      {
        if (!isFirst)
        {
          stringBuilder.Append(",");
        }

        var columnInfo = column.ColumnName.TrimStart(' ').Split(" ");
        var columnName = columnInfo.First().Trim('`');

        var value = columnInfo[1].ToUpper() == "STRING" ? $"\"{column.Value}\"" : column.Value;
        stringBuilder.AppendLine($"\"{columnName}\": {value}");

        isFirst = false;
      }

      stringBuilder.AppendLine("}");

      return stringBuilder.ToString();
    }
  }
}