using System;

namespace Kafka.DotNet.ksqlDB.KSql.Query.Functions
{
  public static class KSqlFunctionsExtensions
  {
    private static string ServerSideOperationErrorMessage = "Operator is not intended for client side operations";

    public static bool Like(this KSqlFunctions kSqlFunctions, string condition, string patternString)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
  }
}