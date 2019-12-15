using System;
using Prism.Mvvm;
using Sample.Domain.Models;

namespace SqlTableDependency.Extensions.WPFSample.ViewModels.Products
{
  public class ProductViewModel : BindableBase
  {
    private readonly Product product;

    public ProductViewModel(Product product)
    {
      this.product = product ?? throw new ArgumentNullException(nameof(product));
    }

    public int Id
    {
      get => product.Id;
    }

    public string Name
    {
      get => product.Name;
      set
      {
        if (product.Name == value)
          return;

        product.Name = value;

        RaisePropertyChanged();
      }
    }

    public void UpdateFrom(Product entity)
    {
      if(Id != entity.Id)
        return;

      Name = entity.Name;
    }
  }
}