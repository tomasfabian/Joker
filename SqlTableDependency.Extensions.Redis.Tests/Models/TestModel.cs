using Joker.Domain;
using Sample.Domain.Models;

namespace SqlTableDependency.Extensions.Redis.Tests.Models
{
  public class TestModel : DomainEntity
  {
    public string Name { get; set; }
  }
}