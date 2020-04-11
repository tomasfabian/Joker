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
  }
}