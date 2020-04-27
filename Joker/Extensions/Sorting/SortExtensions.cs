using System.Collections.Generic;
using System.ComponentModel;
using Joker.Collections;
using Joker.Comparators;

namespace Joker.Extensions.Sorting
{
  public static class SortExtensions
  {
    public static IComparer<TSource> ToComparer<TSource>(this IEnumerable<Sort<TSource>> sorts)
    {
      var comparer = new GenericComparer<TSource>((x, y) =>
      {
        int result = 0;
        
        foreach (var sort in sorts)
        {
          var sortByFunc = sort.SortBy.Compile();

          result = Comparer<object>.Default.Compare(sortByFunc(x), sortByFunc(y));
          
          result *= sort.ListSortDirection == ListSortDirection.Descending ? -1 : 1;
         
          if(result != 0)
            break;
        }

        return result;
      });

      return comparer;
    }
  }
}