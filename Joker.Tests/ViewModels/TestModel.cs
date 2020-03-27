using System;
using Joker.MVVM.Contracts;

namespace Joker.Tests.ViewModels
{
  public class TestModel : IVersion
  {
    public int Id { get; set; }

    public DateTime Timestamp { get; set; }
  }
}