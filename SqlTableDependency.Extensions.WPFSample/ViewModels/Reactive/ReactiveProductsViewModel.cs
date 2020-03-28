using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using Joker.Comparators;
using Joker.Contracts;
using Joker.Domain;
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

    public ReactiveProductsViewModel(
      ISampleDbContext sampleDbContext,
      IReactiveData<Product> reactive,
      IWpfSchedulersFactory schedulersFactory)
      : base(reactive, schedulersFactory)
    {
      this.sampleDbContext = sampleDbContext ?? throw new ArgumentNullException(nameof(sampleDbContext));
      this.schedulersFactory = schedulersFactory ?? throw new ArgumentNullException(nameof(schedulersFactory));

      Comparer = new GenericEqualityComparer<DomainEntity>((x, y) => x.Id == y.Id);
    }

    protected override IScheduler DispatcherScheduler => schedulersFactory.Dispatcher;

    protected override IEnumerable<Product> Query => sampleDbContext.Products.ToList();

    protected override IEqualityComparer<Product> Comparer { get; }

    protected override ProductViewModel CreateViewModel(Product model)
    {
      return new ProductViewModel(model);
    }
  }
}