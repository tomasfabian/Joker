using Joker.Contracts;
using Joker.MVVM.ViewModels;
using OData.Client;
using Sample.Domain.Models;
using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Joker.Platforms.Factories.Schedulers;
using Joker.PubSubUI.Shared.Navigation;

namespace Joker.PubSubUI.Shared.ViewModels.Products
{
  public class ProductsEntityChangesViewModel : EntityChangesViewModel<ProductViewModel>
  {
    #region Fields

    private readonly IPlatformSchedulersFactory schedulersFactory;
    private readonly IODataServiceContextFactory dataServiceContextFactory;
    private readonly IDialogManager dialogManager;

    #endregion

    #region Constructors

    public ProductsEntityChangesViewModel(
      IReactiveListViewModelFactory<ProductViewModel> reactiveListViewModelFactory,
      ITableDependencyStatusProvider statusProvider,
      IPlatformSchedulersFactory schedulersFactory,
      IODataServiceContextFactory dataServiceContextFactory,
      IDialogManager dialogManager) 
      : base(reactiveListViewModelFactory, statusProvider, schedulersFactory.Dispatcher)
    {
      this.schedulersFactory = schedulersFactory;
      this.dataServiceContextFactory = dataServiceContextFactory ?? throw new ArgumentNullException(nameof(dataServiceContextFactory));
      this.dialogManager = dialogManager ?? throw new ArgumentNullException(nameof(dialogManager));

      PropertyChanged += ProductsEntityChangesViewModel_PropertyChanged;
    }

    #endregion

    #region Properties

    #region NewProductName

    private string newProductName;

    public string NewProductName
    {
      get => newProductName;
      set
      {
        SetProperty(ref newProductName, value);

        addNew?.RaiseCanExecuteChanged();
      }
    }

    #endregion

    #region ProductsListViewModel

    public ReactiveProductsViewModel ProductsListViewModel
    {
      get => (ReactiveProductsViewModel)ListViewModel;
      set => ListViewModel = value;
    }

    #endregion

    #endregion

    #region Commands

    #region AddNew

#if !NETCOREAPP && !NETFRAMEWORK && !NETSTANDARD
    private Joker.WinUI3.Shared.Commands.RelayCommand addNew;

    public Microsoft.UI.Xaml.Input.ICommand AddNew => addNew ?? (addNew = new Joker.WinUI3.Shared.Commands.RelayCommand(OnAddNew, OnCanAddNew));
#else
    private Prism.Commands.DelegateCommand addNew;

    public System.Windows.Input.ICommand AddNew => addNew ?? (addNew = new Prism.Commands.DelegateCommand(OnAddNew, OnCanAddNew));
#endif

    private bool OnCanAddNew()
    {
      return !string.IsNullOrEmpty(NewProductName);
    }

    private void OnAddNew()
    {
      var dataServiceContext = dataServiceContextFactory.CreateODataContext();

      var product = new Product { Name = NewProductName };

      dataServiceContext.AddObject("Products", product);
      
      dataServiceContext.SaveChangesAsync()
        .ToObservable()
        .ObserveOn(schedulersFactory.Dispatcher)
        .Subscribe(_ => { NewProductName = null; }, error => dialogManager.ShowMessage($"Failed to add {product.Name}"));
    }

    #endregion

    #endregion

    #region Methods

    private void ProductsEntityChangesViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(ListViewModel))
        NotifyPropertyChanged(nameof(ProductsListViewModel));
    }

    protected override void OnDispose()
    {
      base.OnDispose();
            
      PropertyChanged -= ProductsEntityChangesViewModel_PropertyChanged;
    }

    #endregion
  }
}