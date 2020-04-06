namespace Joker.MVVM.ViewModels
{
  public interface IReactiveListViewModelFactory<TViewModel> : IViewModelFactory<IReactiveListViewModel<TViewModel>> 
    where TViewModel : class, IViewModel
  {
  }
}