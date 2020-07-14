using Joker.Contracts;
using Joker.MVVM.ViewModels;
using OData.Client;
using Joker.Platforms.Factories.Schedulers;
using Joker.PubSubUI.Shared.Navigation;
using Joker.PubSubUI.Shared.ViewModels.Products;

namespace Joker.WinUI3.Shared.ViewModels.Products
{
  public class ProductsEntityChangesWinUIViewModel : ProductsEntityChangesViewModel
  {
    #region Constructors

    public ProductsEntityChangesWinUIViewModel(
      IReactiveListViewModelFactory<ProductViewModel> reactiveListViewModelFactory,
      ITableDependencyStatusProvider statusProvider,
      IPlatformSchedulersFactory schedulersFactory,
      IODataServiceContextFactory dataServiceContextFactory,
      IDialogManager dialogManager)
      : base(reactiveListViewModelFactory, statusProvider, schedulersFactory, dataServiceContextFactory, dialogManager)
    {
    }

    #endregion

    #region Commands

    #region AddNew

#if JOKER_UWP
    private Commands.RelayCommand addNew;

    public new Microsoft.UI.Xaml.Input.ICommand AddNew => addNew ?? (addNew = new Commands.RelayCommand(OnAddNew, OnCanAddNew));
#endif

    #endregion

    #endregion
    
#if JOKER_UWP
    protected override void OnNewProductNameChanged(string newValue)
    {
      addNew?.RaiseCanExecuteChanged();
    }
#endif
  }
}