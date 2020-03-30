using System;
using Joker.Domain;

namespace Joker.MVVM.Tests.Helpers
{
  [Serializable]
  public class TestModel : DomainEntity
  {
    public string Name { get; set; }

    public TestModel Clone()
    {
      return MemberwiseClone() as TestModel;
    }
  }
}