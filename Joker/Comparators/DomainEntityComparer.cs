using System;
using Joker.Domain;

namespace Joker.Comparators
{
  public class DomainEntityComparer : GenericEqualityComparer<IDomainEntity>
  {
    public DomainEntityComparer(Func<IDomainEntity, int> calculateHash = null) 
      : base((x, y) => x.Id == y.Id, calculateHash)
    {
    }
  }
}