using System;
using System.Collections.Generic;

namespace Joker.Comparators
{
  public class GenericComparer<TItem> : IComparer<TItem>
  {
    private readonly Func<TItem, TItem, int> compare = null;

    public GenericComparer(Func<TItem, TItem, int> compare)
    {
      this.compare = compare ?? throw new ArgumentNullException(nameof(compare));
    }

    public int Compare(TItem x, TItem y)
    {
      return compare(x, y);
    }
  }
}