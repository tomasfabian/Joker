using System;
using System.Collections.ObjectModel;

namespace Joker.MVVM.ViewModels
{
  public interface IViewModelsList : IViewModel
  {
    bool IsLoading { get; }
    IObservable<bool> IsLoadingChanged { get; }
  }

  public interface IViewModelsList<TViewModel> : IViewModelsList
    where TViewModel : class, IViewModel
  {
    TViewModel SelectedItem { get; set; }
    IObservable<SelectionChangedEventArgs<TViewModel>> SelectionChanged { get; }
    ReadOnlyObservableCollection<TViewModel> Items { get; }
  }
}