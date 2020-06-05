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

    #endregion
  }
}