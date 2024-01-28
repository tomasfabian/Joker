using Autofac;
using Joker.MVVM.ViewModels;
using Joker.PubSubUI.Shared.ViewModels.Products;

namespace Joker.BlazorApp.Sample.Factories.ViewModels
{
  public class ReactiveListViewModelFactory(ILifetimeScope container)
    : ViewModelsFactory(container), IReactiveListViewModelFactory<ProductViewModel>
  {
    public IReactiveListViewModel<ProductViewModel> Create()
    {
      return Container.Resolve<ReactiveProductsViewModel>();
    }
  }
}