using System;
using System.ComponentModel;

namespace Joker.Collections
{
  public class Sort<TItem>
  {
    public Sort(Func<TItem, IComparable> sortBy, ListSortDirection listSortDirection = ListSortDirection.Ascending)
    {
      SortBy = sortBy;
      ListSortDirection = listSortDirection;
    }

    public Func<TItem, IComparable> SortBy { get; }

    public ListSortDirection ListSortDirection { get; }
  }
}