using System;
using Joker.WPF.Sample.ViewModels.Products;
using Joker.WPF.Sample.ViewModels.Reactive;
using Ninject;
using Ninject.Parameters;
using Sample.Domain.Models;

namespace Joker.WPF.Sample.Factories.ViewModels
{
  public class ViewModelsFactory
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