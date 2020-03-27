using System;
using System.Collections.Generic;

namespace Joker.Comparators
{
  public class GenericEqualityComparer<T> : IEqualityComparer<T>
  {
    private readonly Func<T, T, bool> compare;
    private readonly Func<T, int> calculateHash;

    public GenericEqualityComparer(Func<T, T, bool> comparer, Func<T, int> calculateHash = null)
    {
      compare = comparer ?? throw new ArgumentNullException(nameof(comparer));
      this.calculateHash = calculateHash;
    }

    public bool Equals(T x, T y)
    {
      return OnEquals(x, y);
    }

    protected virtual bool OnEquals(T x, T y)
    {
      return compare(x, y);
    }

    public int GetHashCode(T obj)
    {
      return OnGetHashCode(obj);
    }

    protected virtual int OnGetHashCode(T obj)
    {
      if (calculateHash != null) return calculateHash(obj);

      return obj.ToString().ToLower().GetHashCode();
    }
  }
}