using System;

namespace Kafka.DotNet.ksqlDB.KSql.Linq
{
  public interface IAggregations
  { 
    int Count();
  }

  public interface IAggregations<out TSource> : IAggregations
  {
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
  }
}