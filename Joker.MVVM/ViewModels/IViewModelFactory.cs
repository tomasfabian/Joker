namespace Joker.MVVM.ViewModels
{
  public interface IViewModelFactory<out TViewModel>
    where TViewModel : IViewModel
  {
    TViewModel Create();
  }
}