using System;
using Prism.Mvvm;
using SqlTableDependency.Extensions.WPFSample.ViewModels.Products;

namespace SqlTableDependency.Extensions.WPFSample.ViewModels
{
  public class ShellViewModel : BindableBase, IDisposable
  {
    public ProductsViewModel ProductsViewModel { get; }

    public ShellViewModel(ProductsViewModel productsViewModel)
    {
      ProductsViewModel = productsViewModel;

      ProductsViewModel.Initialize();
    }

    public void Dispose()
    {
      ProductsViewModel?.Dispose();
    }
  }
}