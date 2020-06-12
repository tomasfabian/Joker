using Joker.PubSubUI.Shared.ViewModels.Products;
using Sample.Domain.Models;

namespace Joker.PubSubUI.Shared.Factories.ViewModels
{
  public interface IViewModelsFactory
  {
    ProductViewModel CreateProductViewModel(Product product);
    ProductsEntityChangesViewModel CreateProductsEntityChangesViewModel();
  }
}