using System;
using Joker.MVVM.ViewModels.Domain;

namespace Joker.MVVM.Tests.Helpers
{
  public class TestViewModel : DomainEntityViewModel<TestModel>
  {
    public TestViewModel(TestModel model)
      : base(model)
    {
    }

    public string Name
    {
      get => Model.Name;

      set
      {
        if(value == Model.Name)
          return;

        Model.Name = value;

        NotifyPropertyChanged();
      }
    }

    private string testProperty;

    public string TestProperty
    {
      get => testProperty;

      set => SetProperty(ref testProperty, value, nameof (TestProperty), (oldValue, newValue) => IsDirty = true);
    }

    public bool IsDirty { get; set; }

    protected override void OnUpdateFrom(TestModel updatedModel)
    {
      base.OnUpdateFrom(updatedModel);
      
      Name = updatedModel.Name;
    }

    public override string ToString()
    {
      return $"Id: {Id}, Version: { Timestamp }, Name: {Name}";
    }
  }
}