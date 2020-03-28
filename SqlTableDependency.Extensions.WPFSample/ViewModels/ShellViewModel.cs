using System;
using Joker.Reactive;
using Prism.Mvvm;
using Sample.Data.Context;
using Sample.Domain.Models;
using SqlTableDependency.Extensions.WPFSample.Factories.Schedulers;
using SqlTableDependency.Extensions.WPFSample.ViewModels.Products;
using SqlTableDependency.Extensions.WPFSample.ViewModels.Reactive;

namespace SqlTableDependency.Extensions.WPFSample.ViewModels
{
  public class ShellViewModel : BindableBase, IDisposable
  {
    public ProductsViewModel ProductsViewModel { get; }

    public ShellViewModel(ProductsViewModel productsViewModel)
    {
      ProductsViewModel = productsViewModel;

      ProductsViewModel.Initialize();

      //TODO:
      string connectionString = string.Empty;

      var reactiveProductsViewModel = new ReactiveProductsViewModel(new SampleDbContext(connectionString), new ReactiveData<Product>(), new WpfSchedulersFactory());

      reactiveProductsViewModel.SubscribeToDataChanges();

      reactiveProductsViewModel.Dispose();
    }

    public void Dispose()
    {
      ProductsViewModel?.Dispose();
    }
  }
}