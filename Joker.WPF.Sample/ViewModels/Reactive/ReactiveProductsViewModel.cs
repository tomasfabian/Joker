using Joker.Collections;
using Joker.Comparators;
using Joker.Contracts;
using Joker.Enums;
using Joker.MVVM.ViewModels;
using Joker.WPF.Sample.Factories.Schedulers;
using Joker.WPF.Sample.ViewModels.Products;
using Sample.Data.Context;
using Sample.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Joker.WPF.Sample.Factories.ViewModels;
using OData.Client;

namespace Joker.WPF.Sample.ViewModels.Reactive
{
  public class ReactiveProductsViewModel : ReactiveListViewModel<Product, ProductViewModel>
  {
    private readonly ISampleDbContext sampleDbContext;
    private readonly ViewModelsFactory viewModelsFactory;
    private readonly IWpfSchedulersFactory schedulersFactory;

    public ReactiveProductsViewModel(
      ISampleDbContext sampleDbContext,
      IReactiveData<Product> reactive,
      ViewModelsFactory viewModelsFactory,
      IWpfSchedulersFactory schedulersFactory)
      : base(reactive, schedulersFactory)
    {
      this.sampleDbContext = sampleDbContext;
      this.viewModelsFactory = viewModelsFactory ?? throw new ArgumentNullException(nameof(viewModelsFactory));
      this.schedulersFactory = schedulersFactory ?? throw new ArgumentNullException(nameof(schedulersFactory));

      Comparer = new DomainEntityComparer();
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

        if (sampleDbContext != null)
          return Observable.Start(() => sampleDbContext.Products.ToList(), schedulersFactory.ThreadPool);

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

    #endregion

    #region Methods
    
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
      return product => product.Id != 3;
    }

    #endregion
  }
}