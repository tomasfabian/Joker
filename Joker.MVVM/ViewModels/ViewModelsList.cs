using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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

    #region SelectionChanged

    private readonly ISubject<SelectionChangedEventArgs<TViewModel>> selectionChangedSubject = new ReplaySubject<SelectionChangedEventArgs<TViewModel>>(1);

    /// <summary>
    /// Gets an observable for when selected viewModel changed.
    /// </summary>
    public IObservable<SelectionChangedEventArgs<TViewModel>> SelectionChanged => selectionChangedSubject.AsObservable();

    #endregion

    private TViewModel selectedItem;

    public TViewModel SelectedItem
    {
      get => selectedItem;
      set
      {
        if(value == selectedItem)
          return;

        var oldValue = selectedItem;

        selectedItem = value;

        selectionChangedSubject.OnNext(new SelectionChangedEventArgs<TViewModel>(oldValue, value));

        NotifyPropertyChanged();
      }
    }
  }
}