using System;
using Joker.MVVM.Contracts;
using Joker.MVVM.ViewModels;

namespace Joker.Tests.ViewModels
{
  public class TestViewModel : ViewModel<TestModel>, IVersion
  {
    private readonly TestModel model;

    public TestViewModel(TestModel model)
      : base(model)
    {
      this.model = model ?? throw new ArgumentNullException(nameof(model));
    }

    public DateTime Timestamp => model.Timestamp;
  }
}