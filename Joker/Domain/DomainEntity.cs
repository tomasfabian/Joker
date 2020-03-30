using System;
using System.ComponentModel.DataAnnotations;

namespace Joker.Domain
{
  public abstract class DomainEntity : IDomainEntity
  {
    [Key]
    public int Id { get; set; }

    public DateTime Timestamp { get; set; }
  }
}