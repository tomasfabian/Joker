using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using Joker.Comparators;
using Joker.Contracts;
using Joker.Enums;
using Joker.MVVM.ViewModels;
using Joker.WPF.Sample.Factories.Schedulers;
using Joker.WPF.Sample.ViewModels.Products;
using Sample.Data.Context;
using Sample.Domain.Models;

namespace Joker.WPF.Sample.ViewModels.Reactive
{
  public class ReactiveProductsViewModel : ReactiveListViewModel<Product, ProductViewModel>
  {
    private readonly ISampleDbContext sampleDbContext;
    private readonly IWpfSchedulersFactory schedulersFactory;

    public ReactiveProductsViewModel(
      ISampleDbContext sampleDbContext,
      IReactiveData<Product> reactive,
      IWpfSchedulersFactory schedulersFactory)
      : base(reactive, schedulersFactory)
    {
      this.sampleDbContext = sampleDbContext;
      this.schedulersFactory = schedulersFactory ?? throw new ArgumentNullException(nameof(schedulersFactory));

      Comparer = new DomainEntityComparer();
    }

    protected override IScheduler DispatcherScheduler => schedulersFactory.Dispatcher;

    protected override IEnumerable<Product> Query
    {
      get
      {
        if(sampleDbContext != null)
          return sampleDbContext.Products.ToList();

        return new[] {new Product() {Id = 1}};
      }
    }

    protected override IEqualityComparer<Product> Comparer { get; }

    protected override IComparable GetId(Product model)
    {
      return model.Id;
    }

    protected override ProductViewModel CreateViewModel(Product model)
    {
      return new ProductViewModel(model);
    }

    protected override Product GetModel(EntityChange<Product> entityChange)
    {
      return entityChange.Entity.Clone();
    }
  }
}