using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Windows.Input;
using Joker.Collections;
using Joker.Comparators;
using Joker.Contracts;
using Joker.Enums;
using Joker.Extensions;
using Joker.Extensions.Disposables;
using Joker.MVVM.ViewModels;
using Joker.Platforms.Factories.Schedulers;
using Joker.PubSubUI.Shared.Factories.ViewModels;
using OData.Client;
using Prism.Commands;
using Sample.Domain.Models;

namespace Joker.PubSubUI.Shared.ViewModels.Products
{
  public class ReactiveProductsViewModel : ReactiveListViewModel<Product, ProductViewModel>
  {
    private readonly IViewModelsFactory viewModelsFactory;
    private readonly IPlatformSchedulersFactory schedulersFactory;

    public ReactiveProductsViewModel(
      IReactiveData<Product> reactive,
      IViewModelsFactory viewModelsFactory,
      IPlatformSchedulersFactory schedulersFactory)
      : base(reactive, schedulersFactory)
    {
      this.viewModelsFactory = viewModelsFactory ?? throw new ArgumentNullException(nameof(viewModelsFactory));
      this.schedulersFactory = schedulersFactory ?? throw new ArgumentNullException(nameof(schedulersFactory));

      Comparer = new DomainEntityComparer();
      Init();
    }

    #region Properties

    protected override IScheduler DispatcherScheduler => schedulersFactory.Dispatcher;

    protected override IObservable<IEnumerable<Product>> Query
    {
      get
      {
        ODataServiceContext context = null;

        try
        {
          context = new ODataServiceContextFactory().CreateODataContext();
        }
        catch (Exception e)
        {
          Console.WriteLine(e);
        }

        if(context != null)
          return context.Products.ExecuteAsync().ToObservable();

        //Sample
        return Observable.Create<IEnumerable<Product>>(o =>
        {
          var products = new[]
          {
            new Product() {Id = 1, Name = "Product 1"}
          };
          o.OnNext(products);

          o.OnCompleted();

          return Disposable.Empty;
        }).Delay(TimeSpan.FromSeconds(2));
      }
    }

    protected override IEqualityComparer<Product> Comparer { get; }

    private string filter;

    public string Filter
    {
      get => filter;
      set
      {
        SetProperty(ref filter, value);

        clearFilter?.RaiseCanExecuteChanged();
      }
    }

    #endregion

    #region Commands

    #region ClearFilter

    private DelegateCommand clearFilter;

    public ICommand ClearFilter => clearFilter ?? (clearFilter = new DelegateCommand(OnClearFilter, OnCanClearFilter));

    private bool OnCanClearFilter()
    {
      return !Filter.IsNullOrEmpty();
    }

    private void OnClearFilter()
    {
      Filter = null;
    }

    #endregion

    #endregion

    #region Methods

    private IDisposable selectionChangedSubscription;
    private readonly CompositeDisposable disposable = new CompositeDisposable();

    private void Init()
    {
      selectionChangedSubscription =
        SelectionChanged.Where(c => c != null).Subscribe(c =>
          {
            if(c.OldValue != null)
              c.OldValue.IsActive = false;
            
            if(c.NewValue != null)
              c.NewValue.IsActive = true;
          });
      
      Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
          h => PropertyChanged += h,
          h => PropertyChanged -= h)
        .Where(c => c.EventArgs.PropertyName == nameof(Filter))
        .Throttle(TimeSpan.FromMilliseconds(200))
        .ObserveOn(schedulersFactory.Dispatcher)
        .Subscribe(c => SubscribeToDataChanges())
        .DisposeWith(disposable);
    }

    protected override IComparable GetId(Product model)
    {
      return model.Id;
    }

    protected override ProductViewModel CreateViewModel(Product model)
    {
      return viewModelsFactory.CreateProductViewModel(model);
    }

    protected override Product GetModel(EntityChange<Product> entityChange)
    {
      return entityChange.Entity.Clone();
    }

    protected override Sort<ProductViewModel>[] OnCreateSortDescriptions()
    {
      var sortByName = new Sort<ProductViewModel>(c => c.Name, ListSortDirection.Descending);

      return new [] {sortByName};
    }

    protected override Func<Product, bool> OnCreateModelsFilter()
    {
      return product => Filter.IsNullOrEmpty() || (!product.Name.IsNullOrEmpty() && product.Name.ToLower().Contains(Filter.ToLower()));
    }

    protected override Action<Product, ProductViewModel> UpdateViewModel()
    {
      return (p, vm) => vm.UpdateFrom(p);
    }

    protected override void OnEntitiesLoaded()
    {
      SelectedItem = Items.FirstOrDefault();
    }
    
    protected override void OnDispose()
    {
      base.OnDispose();

      using (disposable)
      using (selectionChangedSubscription)
      {
      }
    }

    #endregion
  }
}