using System;
using System.Collections.ObjectModel;
using System.Linq;
using Joker.PubSubUI.Shared.ViewModels.Products;
using Sample.Domain.Models;

namespace Joker.PubSubUI.Shared.DesignTime.Products
{
  public class ReactiveProductsDesignViewModel
  {
    public ReactiveProductsDesignViewModel()
    {
      var products = Enumerable.Range(1, 22).Select(i => new ProductViewModel(new Product {Name = $"Product {i}"}));

      Items = new ObservableCollection<ProductViewModel>(products);
      
      SelectedItem = Items.First();
    }
    
    public bool IsLoading { get; set; } = true;
    
    public string Filter { get; set; } = "Product";

    public ObservableCollection<ProductViewModel> Items { get; set; }

    public ProductViewModel SelectedItem { get; set; }
  }
}