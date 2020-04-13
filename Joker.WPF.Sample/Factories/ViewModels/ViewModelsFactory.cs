using System;
using Joker.WPF.Sample.ViewModels.Products;
using Ninject;
using Ninject.Parameters;
using Sample.Domain.Models;

namespace Joker.WPF.Sample.Factories.ViewModels
{
  public class ViewModelsFactory
  {
    private readonly IKernel kernel;

    public ViewModelsFactory(IKernel kernel)
    {
      this.kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
    }

    public ProductViewModel CreateProductViewModel(Product product)
    {
      return kernel.Get<ProductViewModel>(new ConstructorArgument(nameof(product), product));
    }
  }
}