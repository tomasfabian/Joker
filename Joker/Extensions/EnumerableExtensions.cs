using System;
using System.Collections.Generic;
using System.Linq;

namespace Joker.Extensions
{
  public static class EnumerableExtensions
  {
    #region IsOneOfFollowing

    public static bool IsOneOfFollowing<TItem>(this TItem item, params TItem[] allowedValues)
    {
      return allowedValues.Any(c => c.Equals(item));
    }

    #endregion

    #region Consume

    public static void Consume<TItem>(this IEnumerable<TItem> source)
    {
      if (source == null) throw new ArgumentNullException(nameof(source));

      foreach (var _ in source)
      {
      }
    }

    #endregion

    #region ForEach

    public static void ForEach<TItem>(this IEnumerable<TItem> source, Action<TItem> action)
    {
      if (source == null) throw new ArgumentNullException(nameof(source));

      foreach (var item in source)
      {
        action(item);
      }
    }
    public static void ForEach<TItem>(this IEnumerable<TItem> source, Action<TItem, long> action)
    {
      if (source == null) throw new ArgumentNullException(nameof(source));

      long index = 0;

      foreach (var item in source)
      {
        action(item, ++index);
      }
    }

    #endregion

    #region Do

    public static IEnumerable<TItem> Do<TItem>(this IEnumerable<TItem> source, Action<TItem> action)
    {
      if (source == null) throw new ArgumentNullException(nameof(source));

      foreach (var item in source)
      {
        action(item);

        yield return item;
      }
    }

    #endregion

    #region ToEnumerable

    public static IEnumerable<TItem> ToEnumerable<TItem>(this TItem item)
    {
      yield return item;
    }

    #endregion
  }
}