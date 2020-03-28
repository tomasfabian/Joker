using System;
using System.ComponentModel.DataAnnotations;
using Joker.MVVM.Contracts;

namespace Joker.Domain
{
  public class DomainEntity : IVersion
  {
    [Key]
    public int Id { get; set; }

    public DateTime Timestamp { get; set; }
  }
}