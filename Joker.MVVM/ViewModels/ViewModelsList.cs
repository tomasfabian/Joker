using System.Collections.Generic;
using System.Collections.ObjectModel;
using Joker.MVVM.Contracts;

namespace Joker.MVVM.ViewModels
{
  public class ViewModelsList<TViewModel> : ViewModel
    where TViewModel : ViewModel, IVersion
  {
    private readonly ObservableCollection<TViewModel> viewModels = new ObservableCollection<TViewModel>();

    protected ObservableCollection<TViewModel> ViewModels => viewModels;

    private ReadOnlyObservableCollection<TViewModel> readOnlyViewModels;

    public ReadOnlyObservableCollection<TViewModel> Items => readOnlyViewModels ?? (readOnlyViewModels = new ReadOnlyObservableCollection<TViewModel>(viewModels));

  }
}