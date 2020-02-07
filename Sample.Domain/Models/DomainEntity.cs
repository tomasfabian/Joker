using System.ComponentModel.DataAnnotations;

namespace Sample.Domain.Models
{
  public class DomainEntity
  {
    [Key]
    public int Id { get; set; }
  }
}