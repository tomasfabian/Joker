using Autofac;
using Joker.MVVM.ViewModels;
using Joker.PubSubUI.Shared.ViewModels.Products;

namespace Joker.BlazorApp.Sample.Factories.ViewModels
{
  public class ReactiveListViewModelFactory : ViewModelsFactory, IReactiveListViewModelFactory<ProductViewModel>
  {
    public ReactiveListViewModelFactory(ILifetimeScope container) 
      : base(container)
    {
    }
    
    public IReactiveListViewModel<ProductViewModel> Create()
    {
      return Container.Resolve<ReactiveProductsViewModel>();
    }
  }
}