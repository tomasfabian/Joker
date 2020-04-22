using Joker.MVVM.ViewModels;
using Joker.WPF.Sample.DesignTime.Reactive;

namespace Joker.WPF.Sample.DesignTime
{
  public class ShellDesignViewModel : ViewModel
  {
    private ProductsEntityChangesDesignViewModel entityChangesViewModel;

    public ShellDesignViewModel()
    {
      EntityChangesViewModel = new ProductsEntityChangesDesignViewModel();
    }

    public ProductsEntityChangesDesignViewModel EntityChangesViewModel
    {
      get => entityChangesViewModel;
      set => SetProperty(ref entityChangesViewModel, value);
    }
  }
}