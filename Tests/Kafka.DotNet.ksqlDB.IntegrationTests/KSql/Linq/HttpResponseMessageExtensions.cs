﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Extensions;

namespace Kafka.DotNet.ksqlDB.IntegrationTests.KSql.Linq
{
  internal static class HttpResponseMessageExtensions
  {
    public static bool IsSuccess(this HttpResponseMessage httpResponseMessage)
    {
      try
      {
        if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
        {
          var responsesObject = httpResponseMessage.ToStatementResponses();

          var isSuccess = responsesObject != null && responsesObject.All(c => c.CommandStatus.Status == "SUCCESS");

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