using System;
using System.Configuration;
using Joker.Reactive;
using Joker.WPF.Sample.Factories.Schedulers;
using Joker.WPF.Sample.ViewModels.Products;
using Joker.WPF.Sample.ViewModels.Reactive;
using Prism.Mvvm;
using Sample.Data.Context;
using Sample.Domain.Models;

namespace Joker.WPF.Sample.ViewModels
{
  public class ShellViewModel : BindableBase, IDisposable
  {
    public ProductsViewModel ProductsViewModel { get; }

    public ShellViewModel(ProductsViewModel productsViewModel)
    {
      ProductsViewModel = productsViewModel;

      ProductsViewModel.Initialize();

      string connectionString = ConfigurationManager.ConnectionStrings["FargoEntities"].ConnectionString;

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