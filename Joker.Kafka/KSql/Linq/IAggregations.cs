using System;

namespace Kafka.DotNet.ksqlDB.KSql.Linq
{
  public interface IAggregations
  { 
    int Count();
  }

  public interface IAggregations<out TSource> : IAggregations
  {
    int Avg(Func<TSource, int?> selector);
    long Avg(Func<TSource, long?> selector);
    decimal Avg(Func<TSource, decimal?> selector);
    decimal Avg(Func<TSource, float?> selector);
    decimal Avg(Func<TSource, double?> selector);
    int Avg(Func<TSource, int> selector);
    long Avg(Func<TSource, long> selector);
    decimal Avg(Func<TSource, decimal> selector);
    decimal Avg(Func<TSource, float> selector);
    decimal Avg(Func<TSource, double> selector);
    
    int Min(Func<TSource, int?> selector);
    long Min(Func<TSource, long?> selector);
    decimal Min(Func<TSource, decimal?> selector);
    decimal Min(Func<TSource, float?> selector);
    decimal Min(Func<TSource, double?> selector);
    int Min(Func<TSource, int> selector);
    long Min(Func<TSource, long> selector);
    decimal Min(Func<TSource, decimal> selector);
    decimal Min(Func<TSource, float> selector);
    decimal Min(Func<TSource, double> selector);
    
    int Max(Func<TSource, int?> selector);
    long Max(Func<TSource, long?> selector);
    decimal Max(Func<TSource, decimal?> selector);
    decimal Max(Func<TSource, float?> selector);
    decimal Max(Func<TSource, double?> selector);
    int Max(Func<TSource, int> selector);
    long Max(Func<TSource, long> selector);
    decimal Max(Func<TSource, decimal> selector);
    decimal Max(Func<TSource, float> selector);
    decimal Max(Func<TSource, double> selector);
    
    int Sign(Func<TSource, int> selector);
    int Sign(Func<TSource, long> selector);
    int Sign(Func<TSource, decimal> selector);
    int Sign(Func<TSource, float> selector);
    int Sign(Func<TSource, double> selector);

    int Sum(Func<TSource, int?> selector);
    long Sum(Func<TSource, long?> selector);
    decimal Sum(Func<TSource, decimal?> selector);
    decimal Sum(Func<TSource, float?> selector);
    decimal Sum(Func<TSource, double?> selector);
    int Sum(Func<TSource, int> selector);
    long Sum(Func<TSource, long> selector);
    decimal Sum(Func<TSource, decimal> selector);
    decimal Sum(Func<TSource, float> selector);
    decimal Sum(Func<TSource, double> selector);
    
    int Sqrt(Func<TSource, int> selector);
    long Sqrt(Func<TSource, long> selector);
    decimal Sqrt(Func<TSource, decimal> selector);
    decimal Sqrt(Func<TSource, float> selector);
    decimal Sqrt(Func<TSource, double> selector);
  }
}