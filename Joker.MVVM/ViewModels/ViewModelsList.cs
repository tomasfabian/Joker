using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Joker.Collections;
using Joker.Extensions.Sorting;

namespace Joker.MVVM.ViewModels
{
  public class ViewModelsList<TViewModel> : ViewModel
    where TViewModel : class, IViewModel
  {
    public ViewModelsList()
    {
      ViewModels = new ObservableCollection<TViewModel>();
    }

    private ObservableCollection<TViewModel> viewModels;

    protected ObservableCollection<TViewModel> ViewModels
    {
      get => viewModels;
      set
      {
        if(viewModels == value)
          return;

        viewModels = value;

        Items = new ReadOnlyObservableCollection<TViewModel>(viewModels);

        NotifyPropertyChanged();
      }
    }

    private ReadOnlyObservableCollection<TViewModel> readOnlyViewModels;

    public ReadOnlyObservableCollection<TViewModel> Items
    {
      get => readOnlyViewModels;
      private set
      {
        if(readOnlyViewModels == value)
          return;

        readOnlyViewModels = value;

        NotifyPropertyChanged();
      }
    }

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
        if(Equals(value, selectedItem))
          return;

        var oldValue = selectedItem;

        selectedItem = value;

        selectionChangedSubject.OnNext(new SelectionChangedEventArgs<TViewModel>(oldValue, value));

        NotifyPropertyChanged();
      }
    }

    protected void SetSortDescriptions(IEnumerable<Sort<TViewModel>> sorts)
    {
      SetSortDescriptions(sorts.ToArray());
    }

    protected void SetSortDescriptions(params Sort<TViewModel>[] sorts)
    {
      if(!sorts.Any())
      {
        ViewModels = new ObservableCollection<TViewModel>(ViewModels);

        return;
      }

      var comparer = sorts.ToComparer();

      SortedObservableCollection<TViewModel> sortedObservableCollection = new SortedObservableCollection<TViewModel>(comparer, ViewModels);
      
      ViewModels = sortedObservableCollection;
    }
  }
}