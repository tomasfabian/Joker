using System.ComponentModel;
using Joker.Collections;
using Joker.MVVM.Tests.Helpers;
using Joker.MVVM.ViewModels;

namespace Joker.MVVM.Tests.ViewModels
{
  public class TestModelsViewModel : ViewModelsList<TestViewModel>
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
      var testViewModel = new TestViewModel(testModel);

      ViewModels.Add(testViewModel);
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
  }
}