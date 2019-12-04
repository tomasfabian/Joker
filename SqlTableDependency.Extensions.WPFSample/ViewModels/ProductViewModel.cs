using System;
using Prism.Mvvm;
using Sample.Domain.Models;

namespace SqlTableDependency.Extensions.WPFSample.ViewModels
{
  public class ProductViewModel : BindableBase
  {
    private readonly Product product;

    public ProductViewModel(Product product)
    {
      this.product = product ?? throw new ArgumentNullException(nameof(product));
    }
  }
}