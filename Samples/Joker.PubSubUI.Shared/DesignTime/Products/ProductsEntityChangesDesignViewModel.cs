namespace Joker.PubSubUI.Shared.DesignTime.Products
{
  public class ProductsEntityChangesDesignViewModel
  {
    public ProductsEntityChangesDesignViewModel()
    {
      //ListViewModel = new ReactiveProductsDesignViewModel();
      IsOffline = true;
    }

    public bool IsOffline { get; set; }
    
    public string NewProductName { get; set; } = "New";

    //public ReactiveProductsDesignViewModel ListViewModel { get; set; }
  }
}