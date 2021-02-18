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
    
    public static int Ceil(this KSqlFunctions kSqlFunctions, int input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    public static long Ceil(this KSqlFunctions kSqlFunctions, long input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
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
    
    public static int Floor(this KSqlFunctions kSqlFunctions, int input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    public static long Floor(this KSqlFunctions kSqlFunctions, long input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
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
    
    //TODO:returns BIGINT
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

    #region Random

    public static double Random(this KSqlFunctions kSqlFunctions)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion

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
    
    /// <summary>
    /// Gets an integer representing days since epoch.
    /// </summary>
    /// <returns></returns>
    public static int UnixDate(this KSqlFunctions kSqlFunctions)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Gets the Unix timestamp in milliseconds, represented as a long (BIGINT).
    /// </summary>
    /// <returns></returns>
    public static long UnixTimestamp(this KSqlFunctions kSqlFunctions)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Converts an integer representing days since epoch to a date string using the given format pattern.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="epochDays">The Epoch Day to convert, based on the epoch 1970-01-01</param>
    /// <param name="formatPattern">The format pattern should be in the format expected by java.time.format.DateTimeFormatter.</param>
    /// <returns></returns>
    public static string DateToString(this KSqlFunctions kSqlFunctions, int epochDays, string formatPattern)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Converts a string representation of a date into an integer representing days since epoch using the given format pattern.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="formattedDate">The string representation of a date</param>
    /// <param name="formatPattern">The format pattern should be in the format expected by java.time.format.DateTimeFormatter.</param>
    /// <returns></returns>
    public static int StringToDate(this KSqlFunctions kSqlFunctions, string formattedDate, string formatPattern)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    /// <summary>
    /// Converts a string representation of a date in the given format into the BIGINT value that represents the millisecond timestamp.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="formattedTimestamp">The string representation of a date.</param>
    /// <param name="formatPattern">The format pattern should be in the format expected by java.time.format.DateTimeFormatter.</param>
    /// <returns></returns>
    public static long StringToTimestamp(this KSqlFunctions kSqlFunctions, string formattedTimestamp, string formatPattern)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    //STRINGTOTIMESTAMP
    /// <summary>
    /// Converts a string representation of a date in the given format into the BIGINT value that represents the millisecond timestamp.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="formattedTimestamp">The string representation of a date.</param>
    /// <param name="formatPattern">The format pattern should be in the format expected by java.time.format.DateTimeFormatter.</param>
    /// <param name="timeZone">timeZone is a java.util.TimeZone ID format, for example: "UTC", "America/Los_Angeles", "PST", "Europe/London"</param>
    /// <returns></returns>
    public static long StringToTimestamp(this KSqlFunctions kSqlFunctions, string formattedTimestamp, string formatPattern, string timeZone)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    /// <summary>
    /// Converts a BIGINT millisecond timestamp value into the string representation of the timestamp in the given format.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="epochMilli">Milliseconds since January 1, 1970, 00:00:00 GMT.</param>
    /// <param name="formatPattern">The format pattern should be in the format expected by java.time.format.DateTimeFormatter.</param>
    /// <returns>String representation of the timestamp in the given format.</returns>
    public static string TimestampToString(this KSqlFunctions kSqlFunctions, long epochMilli, string formatPattern)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Converts a BIGINT millisecond timestamp value into the string representation of the timestamp in the given format.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="epochMilli">Milliseconds since January 1, 1970, 00:00:00 GMT.</param>
    /// <param name="formatPattern">The format pattern should be in the format expected by java.time.format.DateTimeFormatter.</param>
    /// <param name="timeZone">timeZone is a java.util.TimeZone ID format, for example: "UTC", "America/Los_Angeles", "PST", "Europe/London"</param>
    /// <returns>String representation of the timestamp in the given format.</returns>
    public static string TimestampToString(this KSqlFunctions kSqlFunctions, long epochMilli, string formatPattern, string timeZone)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion
  }
}