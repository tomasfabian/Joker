using System;
using System.ComponentModel.DataAnnotations;

namespace Joker.Domain
{
  public abstract class DomainEntity : DomainEntity<int>, IDomainEntity
  {
  }

  public abstract class DomainEntity<TKey> : IDomainEntity<TKey>
  {
    [Key]
    public TKey Id { get; set; }

    public DateTime Timestamp { get; set; }
  }
}