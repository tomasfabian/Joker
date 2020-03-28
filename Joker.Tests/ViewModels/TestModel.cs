using System;
using Joker.MVVM.Contracts;

namespace Joker.MVVM.Tests.ViewModels
{
  public class TestModel : IVersion
  {
    public int Id { get; set; }

    public DateTime Timestamp { get; set; }

    public string Name { get; set; }

    public TestModel Clone()
    {
      return MemberwiseClone() as TestModel;
    }
  }
}