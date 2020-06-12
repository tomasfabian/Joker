using Joker.MVVM.ViewModels;
using Joker.PubSubUI.Shared.ViewModels.Products;
using Ninject;

namespace Joker.PubSubUI.Shared.Factories.ViewModels
{
  public class ReactiveListViewModelFactory : ViewModelsFactory, IReactiveListViewModelFactory<ProductViewModel>
  {
    public ReactiveListViewModelFactory(IKernel kernel) 
      : base(kernel)
    {
    }
    
    public IReactiveListViewModel<ProductViewModel> Create()
    {
      return Kernel.Get<ReactiveProductsViewModel>();
    }
  }
}