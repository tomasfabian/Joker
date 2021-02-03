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

    public static string Trim(this KSqlFunctions kSqlFunctions, string input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    public static string LPad(this KSqlFunctions kSqlFunctions, string input, int length, string padding)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    public static string RPad(this KSqlFunctions kSqlFunctions, string input, int length, string padding)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    public static string Substring(this KSqlFunctions kSqlFunctions, string input, int position, int length)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
  }
}