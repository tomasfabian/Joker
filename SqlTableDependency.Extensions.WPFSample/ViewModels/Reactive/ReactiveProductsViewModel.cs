using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using Joker.Comparators;
using Joker.Domain;
using Joker.MVVM.Contracts;
using Joker.MVVM.ViewModels;
using Sample.Data.Context;
using Sample.Domain.Models;
using SqlTableDependency.Extensions.WPFSample.Factories.Schedulers;
using SqlTableDependency.Extensions.WPFSample.ViewModels.Products;

namespace SqlTableDependency.Extensions.WPFSample.ViewModels.Reactive
{
  public class ReactiveProductsViewModel : ReactiveListViewModel<Product, ProductViewModel>
  {
    private readonly ISampleDbContext sampleDbContext;
    private readonly IWpfSchedulersFactory schedulersFactory;

    public ReactiveProductsViewModel(ISampleDbContext sampleDbContext, IReactive<Product> reactive, IWpfSchedulersFactory schedulersFactory)
      : base(reactive, schedulersFactory)
    {
      this.sampleDbContext = sampleDbContext ?? throw new ArgumentNullException(nameof(sampleDbContext));
      this.schedulersFactory = schedulersFactory ?? throw new ArgumentNullException(nameof(schedulersFactory));

      Comparer = new GenericEqualityComparer<DomainEntity>((x, y) => x.Id == y.Id);
    }

    #region Properties

    protected override IScheduler DispatcherScheduler => schedulersFactory.Dispatcher;

    protected override IEnumerable<Product> Query => sampleDbContext.Products.ToList();

    protected override IEqualityComparer<Product> Comparer { get; }
    
    private bool isOffline = true;

    public bool IsOffline
    {
      get => isOffline;
      set
      {
        if(isOffline == value)
          return;

        isOffline = value;

        NotifyPropertyChanged();
      }
    }

    #endregion

    #region Methods

    protected override ProductViewModel CreateViewModel(Product model)
    {
      return new ProductViewModel(model);
    }

    public void Initialize()
    {
      SubscribeToDataChanges();
    }

    protected override void OnDispose()
    {
      base.OnDispose();

    }

    #endregion
  }
}