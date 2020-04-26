using Joker.Contracts;

namespace Joker.Domain
{
  public interface IDomainEntity : IDomainEntity<int>
  {
  }

  public interface IDomainEntity<TKey> : IVersion
  {
    TKey Id { get; set; }
  }
}