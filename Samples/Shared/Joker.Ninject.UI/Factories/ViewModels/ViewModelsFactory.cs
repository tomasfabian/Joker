using System;
using Joker.PubSubUI.Shared.ViewModels.Products;
using Ninject;
using Ninject.Parameters;
using Sample.Domain.Models;

namespace Joker.PubSubUI.Shared.Factories.ViewModels
{
  public class ViewModelsFactory : IViewModelsFactory
  {
    protected readonly IKernel Kernel;

    public ViewModelsFactory(IKernel kernel)
    {
      Kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
    }

    public ProductViewModel CreateProductViewModel(Product product)
    {
      return Kernel.Get<ProductViewModel>(new ConstructorArgument(nameof(product), product));
    }

    public ProductsEntityChangesViewModel CreateProductsEntityChangesViewModel()
    {
      return Kernel.Get<ProductsEntityChangesViewModel>();
    }
  }
}