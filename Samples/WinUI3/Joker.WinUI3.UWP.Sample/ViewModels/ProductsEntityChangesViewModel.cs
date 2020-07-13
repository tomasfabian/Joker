using Joker.Contracts;
using Joker.MVVM.ViewModels;
using Joker.Platforms.Factories.Schedulers;
using Joker.PubSubUI.Shared.Navigation;
using Joker.PubSubUI.Shared.ViewModels.Products;
using OData.Client;

namespace Joker.WinUI3.UWP.Sample.ViewModels
{
  public class ProductsEntityChangesViewModel : PubSubUI.Shared.ViewModels.Products.ProductsEntityChangesViewModel
  {
    public ProductsEntityChangesViewModel(IReactiveListViewModelFactory<ProductViewModel> reactiveListViewModelFactory,
      ITableDependencyStatusProvider statusProvider, IPlatformSchedulersFactory schedulersFactory,
      IODataServiceContextFactory dataServiceContextFactory, IDialogManager dialogManager)
      : base(reactiveListViewModelFactory, statusProvider, schedulersFactory, dataServiceContextFactory, dialogManager)
    {
    }

    private Shared.Commands.RelayCommand addNew;

    public new Microsoft.UI.Xaml.Input.ICommand AddNew =>
      addNew ?? (addNew = new Shared.Commands.RelayCommand(OnAddNew, OnCanAddNew));
  }
}