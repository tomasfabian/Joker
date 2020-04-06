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
  public abstract class ViewModelsList<TModel, TViewModel> : ViewModel, IViewModelsList<TViewModel>
    where TModel : class
    where TViewModel : class, IViewModel
  {
    protected ViewModelsList()
    {
      Init();
    }

    private ObservableCollection<TViewModel> viewModels;

    private ObservableCollection<TViewModel> ViewModels
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

    private Func<TModel, bool> ModelsFilter { get; set; }

    private void Init()
    {
      ViewModels = new ObservableCollection<TViewModel>();

      SetModelFilters();
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
    
    internal bool ApplyModelFilter(TModel model)
    {
      return ModelsFilter == null || ModelsFilter(model);
    }

    internal void SetModelFilters()
    {
      ModelsFilter = OnCreateModelsFilter();
    }

    protected virtual Func<TModel, bool> OnCreateModelsFilter()
    {
      return null;
    } 
    
    protected abstract TViewModel CreateViewModel(TModel model);

    protected bool TryAddViewModelFor(TModel model)
    {
      if (ApplyModelFilter(model))
      {
        TViewModel viewModel = CreateViewModel(model);
      
        ViewModels.Add(viewModel);

        return true;
      }

      return false;
    }

    protected bool RemoveViewModel(TViewModel viewModel)
    {
      return ViewModels.Remove(viewModel);
    }

    protected void ClearViewModels()
    {
      ViewModels.Clear();
    }
  }
}