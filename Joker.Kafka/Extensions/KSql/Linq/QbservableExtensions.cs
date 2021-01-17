using System;
using System.Linq.Expressions;
using System.Reflection;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Linq
{
  public static class QbservableExtensions
  {
    #region Select

    private static MethodInfo? selectTSourceTResult;

    public static MethodInfo SelectTSourceTResult(Type TSource, Type TResult) =>
      (selectTSourceTResult ??
       (selectTSourceTResult = new Func<IQbservable<object>, Expression<Func<object, object>>, IQbservable<object>>(Select).GetMethodInfo().GetGenericMethodDefinition()))
      .MakeGenericMethod(TSource, TResult);

    public static IQbservable<TResult> Select<TSource, TResult>(this IQbservable<TSource> source, Expression<Func<TSource, TResult>> selector)
    {
      if (source == null)
        throw new ArgumentNullException(nameof(source));

      if (selector == null)
        throw new ArgumentNullException(nameof(selector));

      return source.Provider.CreateQuery<TResult>(
        Expression.Call(
          null,
          SelectTSourceTResult(typeof(TSource), typeof(TResult)),
          source.Expression, Expression.Quote(selector)
        ));
    }

    #endregion

    #region Where

    private static MethodInfo? whereTSource;

    public static MethodInfo WhereTSource(Type TSource) =>
      (whereTSource ??
       (whereTSource = new Func<IQbservable<object>, Expression<Func<object, bool>>, IQbservable<object>>(QbservableExtensions.Where).GetMethodInfo().GetGenericMethodDefinition()))
      .MakeGenericMethod(TSource);

    public static IQbservable<TSource> Where<TSource>(this IQbservable<TSource> source, Expression<Func<TSource, bool>> predicate)
    {
      if (source == null)
        throw new ArgumentNullException(nameof(source));

      if (predicate == null)
        throw new ArgumentNullException(nameof(predicate));

      return source.Provider.CreateQuery<TSource>(
        Expression.Call(
          null,
          WhereTSource(typeof(TSource)),
          source.Expression, Expression.Quote(predicate)
        ));
    }

    #endregion
    
    #region Take
    
    private static MethodInfo? takeTSource;

    public static MethodInfo TakeTSource(Type TSource) =>
      (takeTSource ??
       (takeTSource = new Func<IQbservable<object>, int, IQbservable<object>>(Take).GetMethodInfo().GetGenericMethodDefinition()))
      .MakeGenericMethod(TSource);

    public static IQbservable<TSource> Take<TSource>(this IQbservable<TSource> source, int count)
    {
      return source.Provider.CreateQuery<TSource>(
        Expression.Call(
          null,
          TakeTSource(typeof(TSource)),
          source.Expression, Expression.Constant(count)
        ));
    }

    #endregion

    #region ToQueryString

    public static string ToQueryString<TSource>(this IQbservable<TSource> source)
    {
      var ksqlQuery = new KSqlQueryGenerator<TSource>().BuildKSql(source.Expression);

      return ksqlQuery;
    }

    #endregion
  }
}