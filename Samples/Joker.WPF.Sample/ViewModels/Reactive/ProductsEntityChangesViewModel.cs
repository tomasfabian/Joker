using Joker.Contracts;
using Joker.MVVM.ViewModels;
using Joker.WPF.Sample.Factories.Schedulers;
using Joker.WPF.Sample.ViewModels.Products;
using OData.Client;
using Prism.Commands;
using Sample.Domain.Models;
using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Joker.WPF.Sample.ViewModels.Reactive
{
  public class ProductsEntityChangesViewModel : EntityChangesViewModel<ProductViewModel>
  {
    private readonly IPlatformSchedulersFactory schedulersFactory;

    public ProductsEntityChangesViewModel(
      IReactiveListViewModelFactory<ProductViewModel> reactiveListViewModelFactory,
      ITableDependencyStatusProvider statusProvider,
      IPlatformSchedulersFactory schedulersFactory) 
      : base(reactiveListViewModelFactory, statusProvider, schedulersFactory.Dispatcher)
    {
      this.schedulersFactory = schedulersFactory ?? throw new ArgumentNullException(nameof(schedulersFactory));

      PropertyChanged += ProductsEntityChangesViewModel_PropertyChanged;
    }

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

    private DelegateCommand addNew;

    public ICommand AddNew => addNew ?? (addNew = new DelegateCommand(OnAddNew, OnCanAddNew));

    private bool OnCanAddNew()
    {
      return !string.IsNullOrEmpty(NewProductName);
    }

    private void OnAddNew()
    {
      var dataServiceContext = new ODataServiceContextFactory().CreateODataContext();

      var product = new Product { Name = NewProductName };

      dataServiceContext.AddObject("Products", product);
      
      dataServiceContext.SaveChangesAsync()
        .ToObservable()
        .ObserveOn(schedulersFactory.Dispatcher)
        .Subscribe(_ => { NewProductName = null; }, error => MessageBox.Show($"Failed to add {product.Name}"));
    }

    #endregion

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