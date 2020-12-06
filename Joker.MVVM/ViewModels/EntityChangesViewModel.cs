using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Joker.Contracts;
using Joker.Disposables;
using Joker.Extensions.Disposables;
using Joker.Notifications;

namespace Joker.MVVM.ViewModels
{
  public class EntityChangesViewModel<TViewModel> : ViewModel, IDisposable 
    where TViewModel : class, IViewModel
  {
    #region Fields

    private DisposableObject disposable;

    private readonly IReactiveListViewModelFactory<TViewModel> reactiveListViewModelFactory;
    private readonly ITableDependencyStatusProvider statusProvider;
    private readonly IScheduler dispatcherScheduler;

    #endregion

    #region Constructors

    public EntityChangesViewModel(
      IReactiveListViewModelFactory<TViewModel> reactiveListViewModelFactory,
      ITableDependencyStatusProvider statusProvider,
      IScheduler dispatcherScheduler)
    {
      this.reactiveListViewModelFactory = reactiveListViewModelFactory ?? throw new ArgumentNullException(nameof(reactiveListViewModelFactory));
      this.statusProvider = statusProvider ?? throw new ArgumentNullException(nameof(statusProvider));
      this.dispatcherScheduler = dispatcherScheduler ?? throw new ArgumentNullException(nameof(dispatcherScheduler));

      Init();
    }

    #endregion

    #region Properties

    private bool isOffline = true;

    public bool IsOffline
    {
      get => isOffline;
      private set
      {
        if(value == isOffline)
          return;
        
        isOffline = value;

        if (value)
          OnIsOffline();
        else
          OnIsOnline();
        
        NotifyPropertyChanged();
      }
    }

    private IReactiveListViewModel<TViewModel> reactiveListViewModel;

    public IReactiveListViewModel<TViewModel> ListViewModel
    {
      get => reactiveListViewModel;
      set => SetProperty(ref reactiveListViewModel, value);
    }

    #endregion

    #region Methods

    private void Init()
    {
      disposable = DisposableObject.Create(OnDispose);

      statusProvider.WhenStatusChanges
        .ObserveOn(dispatcherScheduler)
        .Where(c => lastStatus == null || c.Timestamp == DateTimeOffset.MinValue || lastStatus.Timestamp <= c.Timestamp)
        .Subscribe(OnStatusChanged)
        .DisposeWith(disposable);
    }

    private VersionedTableDependencyStatus lastStatus;

    private void OnStatusChanged(VersionedTableDependencyStatus status)
    {
      lastStatus = status;

      switch (status.TableDependencyStatus)
      {
        case VersionedTableDependencyStatus.TableDependencyStatuses.WaitingForNotification:
        case VersionedTableDependencyStatus.TableDependencyStatuses.Started:
          IsOffline = false;
          break;

        default:
          IsOffline = true;
          break;
      }
    }

    private void OnIsOnline()
    {
      if(disposable.IsDisposed)
        throw new ObjectDisposedException(GetType().Name);

      ListViewModel = reactiveListViewModelFactory.Create();

      ListViewModel.SubscribeToDataChanges();
    }    

    private void OnIsOffline()
    {
      using (ListViewModel)
      {
        ListViewModel = null;
      }
    }
    
    protected virtual void OnDispose()
    {
      IsOffline = true;
    }

    public void Dispose()
    {
      using (disposable)
      { }
    }

    #endregion
  }
}