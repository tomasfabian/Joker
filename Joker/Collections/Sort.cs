using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Joker.Collections
{
  public class Sort<TItem>
  {
    public Sort(Expression<Func<TItem, IComparable>> sortBy, ListSortDirection listSortDirection = ListSortDirection.Ascending)
    {
      SortBy = sortBy;
      ListSortDirection = listSortDirection;
    }

    public Expression<Func<TItem, IComparable>> SortBy { get; }

    public ListSortDirection ListSortDirection { get; }
  }
}