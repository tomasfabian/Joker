using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Kafka.DotNet.ksqlDB.IntegrationTests.KSql.RestApi;

namespace Kafka.DotNet.ksqlDB.IntegrationTests.KSql.Linq
{
  internal static class HttpResponseMessageExtensions
  {
    public static bool IsSuccess(this HttpResponseMessage httpResponseMessage)
    {
      string responseContent = httpResponseMessage.Content.ReadAsStringAsync().Result;
      
      try
      {
        if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
        {
          var responseObject = JsonSerializer.Deserialize<KSqlDbRestApiClientTests.StatementResponse[]>(responseContent);

          var isSuccess = responseObject != null && responseObject.All(c => c.CommandStatus.Status == "SUCCESS");

          return isSuccess;
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        
        return false;
      }

      return false;
    }
  }
}