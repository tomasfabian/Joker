using System;
using Joker.MVVM.Contracts;
using Joker.MVVM.ViewModels;
using Sample.Domain.Models;

namespace SqlTableDependency.Extensions.WPFSample.ViewModels.Products
{
  public class ProductViewModel : ViewModel<Product>, IVersion
  {
    private readonly Product product;

    public ProductViewModel(Product product)
      : base(product)
    {
      this.product = product ?? throw new ArgumentNullException(nameof(product));
    }

    public int Id => product.Id;

    public DateTime Timestamp => product.Timestamp;

    public string Name
    {
      get => product.Name;
      set
      {
        if (product.Name == value)
          return;

        product.Name = value;

        NotifyPropertyChanged();
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