using System;

namespace Joker.MVVM.ViewModels
{
  public interface IReactiveListViewModel : IViewModelsList, IDisposable
  {
    void SubscribeToDataChanges();
  }

  public interface IReactiveListViewModel<TViewModel> : IViewModelsList<TViewModel>, IReactiveListViewModel
    where TViewModel : class, IViewModel
  {
  }
}