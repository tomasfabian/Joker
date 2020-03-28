using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Joker.Contracts;

namespace Joker.MVVM.ViewModels
{
  public class ViewModelsList<TViewModel> : ViewModel
    where TViewModel : ViewModel, IVersion
  {
    private readonly ObservableCollection<TViewModel> viewModels = new ObservableCollection<TViewModel>();

    protected ObservableCollection<TViewModel> ViewModels => viewModels;

    private ReadOnlyObservableCollection<TViewModel> readOnlyViewModels;

    public ReadOnlyObservableCollection<TViewModel> Items => readOnlyViewModels ?? (readOnlyViewModels = new ReadOnlyObservableCollection<TViewModel>(viewModels));
    
    private bool isLoading;

    public bool IsLoading
    {
      get => isLoading;
      set
      {
        if(value == isLoading)
          return;
        
        isLoading = value;
        
        isLoadingChangedSubject.OnNext(value);

        NotifyPropertyChanged();
      }
    }
    
    #region IsLoadingChanged

    private readonly ISubject<bool> isLoadingChangedSubject = new ReplaySubject<bool>(1);

    public IObservable<bool> IsLoadingChanged => isLoadingChangedSubject.AsObservable();

    #endregion

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