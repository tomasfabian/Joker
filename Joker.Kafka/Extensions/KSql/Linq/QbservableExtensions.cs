using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Linq
{
  public static class QbservableExtensions
  {
    #region Select

    private static MethodInfo? selectTSourceTResult;

    private static MethodInfo SelectTSourceTResult(Type TSource, Type TResult) =>
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

    private static MethodInfo WhereTSource(Type TSource) =>
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

    private static MethodInfo TakeTSource(Type TSource) =>
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
      if (source == null) throw new ArgumentNullException(nameof(source));

      var kStreamSet = source as KStreamSet<TSource>;

      var ksqlQuery = kStreamSet?.KSqlQueryGenerator?.BuildKSql(source.Expression, kStreamSet.QueryContext);

      return ksqlQuery;
    }

    #endregion

    #region ToObservable

    /// <summary>
    /// Runs the ksqlDb query as an observable sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the source stream.</typeparam>
    /// <param name="source">ksqlDb query to convert to an observable sequence.</param>
    /// <returns>The observable sequence whose elements are pushed from the given query.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    public static IObservable<TSource> ToObservable<TSource>(this IQbservable<TSource> source)
    {
      if (source == null) throw new ArgumentNullException(nameof(source));

      return Observable.Defer(() =>
      {
        var streamSet = source as KStreamSet<TSource>;

        var cancellationTokenSource = new CancellationTokenSource();

        var observable = streamSet?.RunStreamAsObservable(cancellationTokenSource);
      
        return observable?.Finally(() => cancellationTokenSource.Cancel());
      });
    }

    #endregion

    #region AsAsyncEnumerable

    /// <summary>
    /// Runs the query as an async-enumerable sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the source stream.</typeparam>
    /// <param name="source">An ksqlDb query to subscribe to.</param>
    /// <returns>An async-enumerable sequence for the query.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    public static IAsyncEnumerable<TSource> ToAsyncEnumerable<TSource>(this IQbservable<TSource> source)
    {
      if (source == null) throw new ArgumentNullException(nameof(source));

      var streamSet = source as KStreamSet<TSource>;
      
      var cancellationTokenSource = new CancellationTokenSource();

      return streamSet?.RunStreamAsAsyncEnumerable(cancellationTokenSource).Finally(() => cancellationTokenSource.Cancel());
    }

    #endregion

    #region Subscribe delegate-based overloads

    /// <summary>
    /// Subscribes an element handler and an exception handler to an qbservable stream.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source stream.</typeparam>
    /// <param name="source">Observable stream to subscribe to.</param>
    /// <param name="onNext">Action to invoke for each element in the qbservable stream.</param>
    /// <param name="onError">Action to invoke upon exceptional termination of the qbservable stream.</param>
    /// <returns><see cref="IDisposable"/> object used to unsubscribe from the qbservable stream.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="onNext"/> or <paramref name="onError"/> is <c>null</c>.</exception>
    public static IDisposable Subscribe<T>(this IQbservable<T> source, Action<T> onNext, Action<Exception> onError)
    {
      if (source == null)
        throw new ArgumentNullException(nameof(source));

      if (onNext == null)
        throw new ArgumentNullException(nameof(onNext));

      if (onError == null)
        throw new ArgumentNullException(nameof(onError));

      return source.Subscribe(new AnonymousObserver<T>(onNext, onError, () => { }));
    }

    /// <summary>
    /// Subscribes an element handler, an exception handler, and a completion handler to an qbservable stream.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source stream.</typeparam>
    /// <param name="source">Observable sequence to subscribe to.</param>
    /// <param name="onNext">Action to invoke for each element in the qbservable stream.</param>
    /// <param name="onError">Action to invoke upon exceptional termination of the qbservable stream.</param>
    /// <param name="onCompleted">Action to invoke upon graceful termination of the qbservable stream.</param>
    /// <returns><see cref="IDisposable"/> object used to unsubscribe from the qbservable stream.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="onNext"/> or <paramref name="onError"/> or <paramref name="onCompleted"/> is <c>null</c>.</exception>
    public static IDisposable Subscribe<T>(this IQbservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted)
    {
      if (source == null)
        throw new ArgumentNullException(nameof(source));

      if (onNext == null)
        throw new ArgumentNullException(nameof(onNext));

      if (onError == null)
        throw new ArgumentNullException(nameof(onError));

      if (onCompleted == null)
        throw new ArgumentNullException(nameof(onCompleted));

      return source.Subscribe(new AnonymousObserver<T>(onNext, onError, onCompleted));
    }

    internal static IDisposable Subscribe<T>(this IQbservable<T> source, Action<T> onNext)
    {
      if (source == null)
        throw new ArgumentNullException(nameof(source));

      if (onNext == null)
        throw new ArgumentNullException(nameof(onNext));

      return source.Subscribe(new AnonymousObserver<T>(onNext, e => throw e, () => {}));
    }

    #endregion

    #region GroupBy
    
    private static MethodInfo? groupByTSourceTKey;

    private static MethodInfo GroupByTSourceTKey(Type TSource, Type TKey) =>
      (groupByTSourceTKey ??
       (groupByTSourceTKey = new Func<IQbservable<object>, Expression<Func<object, object>>, IQbservable<IKSqlGrouping<object, object>>>(GroupBy).GetMethodInfo().GetGenericMethodDefinition()))
      .MakeGenericMethod(TSource, TKey);

    public static IQbservable<IKSqlGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IQbservable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
    {
      if (source == null) throw new ArgumentNullException(nameof(source));
      if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

      return source.Provider.CreateQuery<IKSqlGrouping<TKey, TSource>>(
        Expression.Call(
          null,
          GroupByTSourceTKey(typeof(TSource), typeof(TKey)),
          source.Expression, Expression.Quote(keySelector))
        );
    }

    #endregion
  }
}