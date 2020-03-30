using Joker.Contracts;

namespace Joker.Domain
{
  public interface IDomainEntity : IVersion
  {
    int Id { get; set; }
  }
}