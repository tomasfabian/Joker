namespace Joker.WPF.Sample.DesignTime.Reactive
{
  public class ProductsEntityChangesDesignViewModel
  {
    public ProductsEntityChangesDesignViewModel()
    {
      ListViewModel = new ReactiveProductsDesignViewModel();
      IsOffline = true;
    }

    public bool IsOffline { get; set; } = false;
    
    public string NewProductName { get; set; } = "New";

    public ReactiveProductsDesignViewModel ListViewModel { get; set; }
  }
}