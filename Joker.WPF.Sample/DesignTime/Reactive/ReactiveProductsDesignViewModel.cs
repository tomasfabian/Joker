using System.Collections.ObjectModel;
using System.Linq;
using Joker.WPF.Sample.ViewModels.Products;
using Sample.Domain.Models;

namespace Joker.WPF.Sample.DesignTime.Reactive
{
  public class ReactiveProductsDesignViewModel
  {
    public ReactiveProductsDesignViewModel()
    {
      Items = new ObservableCollection<ProductViewModel>
      {
        new ProductViewModel(new Product { Name = "Product" })
      };

      SelectedItem = Items.First();
    }
    
    public bool IsLoading { get; set; } = true;
    
    public string Filter { get; set; } = "Product";

    public ObservableCollection<ProductViewModel> Items { get; set; }

    public ProductViewModel SelectedItem { get; set; }
  }
}