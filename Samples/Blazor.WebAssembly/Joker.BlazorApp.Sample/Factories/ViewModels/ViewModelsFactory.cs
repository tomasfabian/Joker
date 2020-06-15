using System;
using Autofac;
using Autofac.Core;
using Joker.PubSubUI.Shared.Factories.ViewModels;
using Joker.PubSubUI.Shared.ViewModels.Products;
using Sample.Domain.Models;

namespace Joker.BlazorApp.Sample.Factories.ViewModels
{
  public class ViewModelsFactory : IViewModelsFactory
  {
    protected readonly ILifetimeScope Container;

    public ViewModelsFactory(ILifetimeScope container)
    {
      Container = container ?? throw new ArgumentNullException(nameof(container));
    }

    public ProductViewModel CreateProductViewModel(Product product)
    {
      return Container.Resolve<ProductViewModel>(new ResolvedParameter(
        (pi, ctx) => pi.Name == nameof(product),
        (pi, ctx) => product));
    }

    public ProductsEntityChangesViewModel CreateProductsEntityChangesViewModel()
    {
      return Container.Resolve<ProductsEntityChangesViewModel>();
    }
  }
}