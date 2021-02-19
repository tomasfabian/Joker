using System;
using System.Collections.Generic;

namespace Kafka.DotNet.ksqlDB.KSql.Linq
{
  public interface IAggregations
  { 
    int Count();
    long LongCount();
  }

  public interface IAggregations<out TSource> : IAggregations
  {
    #region Avg

    /// <summary>
    /// Returns the average value of the column computed as the sum divided by the count. Applicable only to numeric types.
    /// </summary>
    int Avg(Func<TSource, int?> selector);

    /// <summary>
    /// Returns the average value of the column computed as the sum divided by the count. Applicable only to numeric types.
    /// </summary>
    long Avg(Func<TSource, long?> selector);

    /// <summary>
    /// Returns the average value of the column computed as the sum divided by the count. Applicable only to numeric types.
    /// </summary>
    decimal Avg(Func<TSource, decimal?> selector);

    /// <summary>
    /// Returns the average value of the column computed as the sum divided by the count. Applicable only to numeric types.
    /// </summary>
    decimal Avg(Func<TSource, float?> selector);

    /// <summary>
    /// Returns the average value of the column computed as the sum divided by the count. Applicable only to numeric types.
    /// </summary>
    decimal Avg(Func<TSource, double?> selector);

    /// <summary>
    /// Returns the average value of the column computed as the sum divided by the count. Applicable only to numeric types.
    /// </summary>
    /// <returns>Computed average of column with type int</returns>
    int Avg(Func<TSource, int> selector);

    /// <summary>
    /// Returns the average value of the column computed as the sum divided by the count. Applicable only to numeric types.
    /// </summary>
    /// <returns>Computed average of column with type long</returns>
    long Avg(Func<TSource, long> selector);

    /// <summary>
    /// Returns the average value of the column computed as the sum divided by the count. Applicable only to numeric types.
    /// </summary>
    decimal Avg(Func<TSource, float> selector);

    /// <summary>
    /// Returns the average value of the column computed as the sum divided by the count. Applicable only to numeric types.
    /// </summary>
    /// <returns>Computed average of column with type double</returns>
    decimal Avg(Func<TSource, double> selector);

    /// <summary>
    /// Returns the average value of the column computed as the sum divided by the count. Applicable only to numeric types.
    /// </summary>
    decimal Avg(Func<TSource, decimal> selector);
    

    #endregion
    
    int Count(Func<TSource, object> selector);
    long LongCount(Func<TSource, object> selector);
   
    int CountDistinct(Func<TSource, object> selector);
    long LongCountDistinct(Func<TSource, object> selector);
    
    string[] CollectList(Func<TSource, string> selector);
    short[] CollectList(Func<TSource, short> selector);
    int[] CollectList(Func<TSource, int> selector);
    long[] CollectList(Func<TSource, long> selector);
    float[] CollectList(Func<TSource, float> selector);
    double[] CollectList(Func<TSource, double> selector);
    decimal[] CollectList(Func<TSource, decimal> selector);

    string[] CollectSet(Func<TSource, string> selector);
    short[] CollectSet(Func<TSource, short> selector);
    int[] CollectSet(Func<TSource, int> selector);
    long[] CollectSet(Func<TSource, long> selector);
    float[] CollectSet(Func<TSource, float> selector);
    double[] CollectSet(Func<TSource, double> selector);
    decimal[] CollectSet(Func<TSource, decimal> selector);

    int Min(Func<TSource, int?> selector);
    long Min(Func<TSource, long?> selector);
    decimal Min(Func<TSource, decimal?> selector);
    decimal Min(Func<TSource, float?> selector);
    decimal Min(Func<TSource, double?> selector);
    int Min(Func<TSource, int> selector);
    long Min(Func<TSource, long> selector);
    decimal Min(Func<TSource, float> selector);
    decimal Min(Func<TSource, double> selector);
    decimal Min(Func<TSource, decimal> selector);
    
    int Max(Func<TSource, int?> selector);
    long Max(Func<TSource, long?> selector);
    decimal Max(Func<TSource, decimal?> selector);
    decimal Max(Func<TSource, float?> selector);
    decimal Max(Func<TSource, double?> selector);
    int Max(Func<TSource, int> selector);
    long Max(Func<TSource, long> selector);
    decimal Max(Func<TSource, float> selector);
    decimal Max(Func<TSource, double> selector);
    decimal Max(Func<TSource, decimal> selector);
    
    /// <summary>
    /// Build a value-to-count histogram of input Strings.
    /// </summary>
    /// <param name="selector"></param>
    /// <returns>Returns a map of each distinct String from the input Stream or Table and how many times each occurs.
    /// This version limits the size of the resultant Map to 1000 entries. Any entries added beyond this limit
    /// will be ignored.</returns>
    IDictionary<string, long> Histogram(Func<TSource, string> selector);
    
    int Sum(Func<TSource, int?> selector);
    long Sum(Func<TSource, long?> selector);
    decimal Sum(Func<TSource, decimal?> selector);
    decimal Sum(Func<TSource, float?> selector);
    decimal Sum(Func<TSource, double?> selector);
    int Sum(Func<TSource, int> selector);
    long Sum(Func<TSource, long> selector);
    decimal Sum(Func<TSource, float> selector);
    decimal Sum(Func<TSource, double> selector);
    decimal Sum(Func<TSource, decimal> selector);

    short[] TopK(Func<TSource, short> selector, int k);
    int[] TopK(Func<TSource, int> selector, int k);
    long[] TopK(Func<TSource, long> selector, int k);
    float[] TopK(Func<TSource, float> selector, int k);
    double[] TopK(Func<TSource, double> selector, int k);
    decimal[] TopK(Func<TSource, decimal> selector, int k);

    short[] TopKDistinct(Func<TSource, short> selector, int k);
    int[] TopKDistinct(Func<TSource, int> selector, int k);
    long[] TopKDistinct(Func<TSource, long> selector, int k);
    float[] TopKDistinct(Func<TSource, float> selector, int k);
    double[] TopKDistinct(Func<TSource, double> selector, int k);
    decimal[] TopKDistinct(Func<TSource, decimal> selector, int k);

    string EarliestByOffset(Func<TSource, string> selector);
    int EarliestByOffset(Func<TSource, int> selector);
    long EarliestByOffset(Func<TSource, long> selector);
    float EarliestByOffset(Func<TSource, float> selector);
    double EarliestByOffset(Func<TSource, double> selector);
    decimal EarliestByOffset(Func<TSource, decimal> selector);

    string EarliestByOffsetAllowNulls(Func<TSource, string> selector);
    int? EarliestByOffsetAllowNulls(Func<TSource, int?> selector);
    long? EarliestByOffsetAllowNulls(Func<TSource, long?> selector);
    float? EarliestByOffsetAllowNulls(Func<TSource, float?> selector);
    double? EarliestByOffsetAllowNulls(Func<TSource, double?> selector);
    decimal? EarliestByOffsetAllowNulls(Func<TSource, decimal?> selector);

    string[] EarliestByOffset(Func<TSource, string> selector, int earliestN);
    int[] EarliestByOffset(Func<TSource, int> selector, int earliestN);
    long[] EarliestByOffset(Func<TSource, long> selector, int earliestN);
    float[] EarliestByOffset(Func<TSource, float> selector, int earliestN);
    double[] EarliestByOffset(Func<TSource, double> selector, int earliestN);
    decimal[] EarliestByOffset(Func<TSource, decimal> selector, int earliestN);

    string[] EarliestByOffsetAllowNulls(Func<TSource, string> selector, int earliestN);
    int?[] EarliestByOffsetAllowNulls(Func<TSource, int?> selector, int earliestN);
    long?[] EarliestByOffsetAllowNulls(Func<TSource, long?> selector, int earliestN);
    float?[] EarliestByOffsetAllowNulls(Func<TSource, float?> selector, int earliestN);
    double?[] EarliestByOffsetAllowNulls(Func<TSource, double?> selector, int earliestN);
    decimal?[] EarliestByOffsetAllowNulls(Func<TSource, decimal?> selector, int earliestN);
    string LatestByOffset(Func<TSource, string> selector);
    int LatestByOffset(Func<TSource, int> selector);
    long LatestByOffset(Func<TSource, long> selector);
    float LatestByOffset(Func<TSource, float> selector);
    double LatestByOffset(Func<TSource, double> selector);
    decimal LatestByOffset(Func<TSource, decimal> selector);

    string LatestByOffsetAllowNulls(Func<TSource, string> selector);
    int? LatestByOffsetAllowNulls(Func<TSource, int?> selector);
    long? LatestByOffsetAllowNulls(Func<TSource, long?> selector);
    float? LatestByOffsetAllowNulls(Func<TSource, float?> selector);
    double? LatestByOffsetAllowNulls(Func<TSource, double?> selector);
    decimal? LatestByOffsetAllowNulls(Func<TSource, decimal?> selector);

    string[] LatestByOffset(Func<TSource, string> selector, int latestN);
    int[] LatestByOffset(Func<TSource, int> selector, int latestN);
    long[] LatestByOffset(Func<TSource, long> selector, int latestN);
    float[] LatestByOffset(Func<TSource, float> selector, int latestN);
    double[] LatestByOffset(Func<TSource, double> selector, int latestN);
    decimal[] LatestByOffset(Func<TSource, decimal> selector, int latestN);

    string[] LatestByOffsetAllowNulls(Func<TSource, string> selector, int latestN);
    int?[] LatestByOffsetAllowNulls(Func<TSource, int?> selector, int latestN);
    long?[] LatestByOffsetAllowNulls(Func<TSource, long?> selector, int latestN);
    float?[] LatestByOffsetAllowNulls(Func<TSource, float?> selector, int latestN);
    double?[] LatestByOffsetAllowNulls(Func<TSource, double?> selector, int latestN);
    decimal?[] LatestByOffsetAllowNulls(Func<TSource, decimal?> selector, int latestN);
  }
}