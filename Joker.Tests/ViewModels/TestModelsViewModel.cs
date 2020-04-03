using System;
using System.ComponentModel;
using Joker.Collections;
using Joker.MVVM.Tests.Helpers;
using Joker.MVVM.ViewModels;

namespace Joker.MVVM.Tests.ViewModels
{
  public class TestModelsViewModel : ViewModelsList<TestModel, TestViewModel>
  {
    public TestModelsViewModel()
    {
      var sorts = new []
      {
        new Sort<TestViewModel>(c => c.Name, ListSortDirection.Descending),
        new Sort<TestViewModel>(c => c.Id),
      };

      SetSortDescriptions(sorts);
    }

    public void Add(TestModel testModel)
    {
      TryAddViewModelFor(testModel);
    }

    public void SetAscendingNameSortDescription()
    {
      var sorts = new []
      {
        new Sort<TestViewModel>(c => c.Name),
        new Sort<TestViewModel>(c => c.Id),
      };

      SetSortDescriptions(sorts);
    }

    protected override TestViewModel CreateViewModel(TestModel model)
    {
      return new TestViewModel(model);
    }

    protected override Func<TestModel, bool> OnCreateModelsFilter()
    {
      return model => model.Name != "Filter";
    }
  }
}