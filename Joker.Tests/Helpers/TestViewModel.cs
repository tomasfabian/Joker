using System;
using Joker.MVVM.Contracts;
using Joker.MVVM.ViewModels;

namespace Joker.MVVM.Tests.Helpers
{
  public class TestViewModel : ViewModel<TestModel>, IVersion
  {
    private readonly TestModel model;

    public TestViewModel(TestModel model)
      : base(model)
    {
      this.model = model ?? throw new ArgumentNullException(nameof(model));
    }

    public DateTime Timestamp
    {
      get => model.Timestamp;

      set
      {
        if(value == model.Timestamp)
          return;

        model.Timestamp = value;

        NotifyPropertyChanged();
      }
    }

    public string Name
    {
      get => model.Name;

      set
      {
        if(value == model.Name)
          return;

        model.Name = value;

        NotifyPropertyChanged();
      }
    }

    public void UpdateFrom(TestModel updatedModel)
    {
      if(updatedModel == null || updatedModel.Id != model.Id)
        return;

      Name = updatedModel.Name;
      Timestamp = updatedModel.Timestamp;
    }
  }
}