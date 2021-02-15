using System;

namespace Kafka.DotNet.ksqlDB.KSql.Query.Functions
{
  public static class KSqlFunctionsExtensions
  {
    private static string ServerSideOperationErrorMessage = "Operator is not intended for client side operations";
    
    public static object Dynamic(this KSqlFunctions kSqlFunctions, string functionCall)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #region Numeric

    #region Abs

    public static int Abs(this KSqlFunctions kSqlFunctions, int input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }  
    
    public static long Abs(this KSqlFunctions kSqlFunctions, long input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    public static float Abs(this KSqlFunctions kSqlFunctions, float input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    public static double Abs(this KSqlFunctions kSqlFunctions, double input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    public static decimal Abs(this KSqlFunctions kSqlFunctions, decimal input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion    
    
    #region Ceil
    
    public static float Ceil(this KSqlFunctions kSqlFunctions, float input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    public static double Ceil(this KSqlFunctions kSqlFunctions, double input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    public static decimal Ceil(this KSqlFunctions kSqlFunctions, decimal input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion
    
    #region Floor
    
    public static float Floor(this KSqlFunctions kSqlFunctions, float input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    public static double Floor(this KSqlFunctions kSqlFunctions, double input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    public static decimal Floor(this KSqlFunctions kSqlFunctions, decimal input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion
    
    #region Round
    
    public static float Round(this KSqlFunctions kSqlFunctions, float input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    public static double Round(this KSqlFunctions kSqlFunctions, double input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    public static decimal Round(this KSqlFunctions kSqlFunctions, decimal input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    public static float Round(this KSqlFunctions kSqlFunctions, float input, int scale)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    public static double Round(this KSqlFunctions kSqlFunctions, double input, int scale)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    public static decimal Round(this KSqlFunctions kSqlFunctions, decimal input, int scale)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion
    
    public static double Random(this KSqlFunctions kSqlFunctions)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #region Sign

    public static int Sign(this KSqlFunctions kSqlFunctions, short input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    public static int Sign(this KSqlFunctions kSqlFunctions, int input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    public static int Sign(this KSqlFunctions kSqlFunctions, long input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    public static int Sign(this KSqlFunctions kSqlFunctions, float input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    public static int Sign(this KSqlFunctions kSqlFunctions, double input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    public static int Sign(this KSqlFunctions kSqlFunctions, decimal input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion

    #region Sqrt
    
    //int Sqrt(Func<TSource, int> selector);
    //long Sqrt(Func<TSource, long> selector);
    //decimal Sqrt(Func<TSource, decimal> selector);
    //decimal Sqrt(Func<TSource, float> selector);
    //decimal Sqrt(Func<TSource, double> selector);

    #endregion

    #endregion

    #region String functions

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

    #endregion

    #region Date and time functions
    
    public static string DateToString(this KSqlFunctions kSqlFunctions, int epochDays, string formatPattern)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion
  }
}