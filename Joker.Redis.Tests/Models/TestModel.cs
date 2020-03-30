using Joker.Domain;

namespace Joker.Redis.Tests.Models
{
  public class TestModel : DomainEntity
  {
    public string Name { get; set; }
  }
}