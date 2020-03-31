#region License
// Joker
// Copyright (c) 2019-2020 Tomas Fabian. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Joker.Contracts;
using Joker.Disposables;
using Joker.Enums;
using Joker.Extensions.Cloning;
using Joker.Factories.Schedulers;

namespace Joker.MVVM.ViewModels
{
  public abstract class ReactiveListViewModel<TModel, TViewModel> : ViewModelsList<TViewModel>, IDisposable
    where TModel : class, IVersion
    where TViewModel : class, IViewModel<TModel>
  {
    #region Fields

    private readonly IReactiveData<TModel> reactive;
    private readonly ISchedulersFactory schedulersFactory;
    private DisposableObject disposable;

    #endregion

    #region Constructors

    protected ReactiveListViewModel(IReactiveData<TModel> reactive, ISchedulersFactory schedulersFactory)
    {
      this.reactive = reactive ?? throw new ArgumentNullException(nameof(reactive));
      this.schedulersFactory = schedulersFactory ?? throw new ArgumentNullException(nameof(schedulersFactory));

      Init();
    }

    #endregion

    #region Properties

    protected abstract IScheduler DispatcherScheduler { get; }

    protected abstract IEnumerable<TModel> Query { get; }

    protected virtual IObservable<IList<EntityChange<TModel>>> DataChanges => reactive.WhenDataChanges
      .Buffer(DataChangesBufferTimeSpan, DataChangesBufferCount, schedulersFactory.TaskPool);

    protected virtual TimeSpan DataChangesBufferTimeSpan => TimeSpan.FromMilliseconds(250);

    protected virtual int DataChangesBufferCount => 100;
    
    #endregion

    #region Methods

    private void Init()
    {
      disposable = DisposableObject.Create(OnDispose);
    }

    private SerialDisposable loadEntitiesSubscription;

    private void LoadEntities()
    {
      IsLoading = true;

      if (loadEntitiesSubscription == null)
        loadEntitiesSubscription = new SerialDisposable();
      else
        loadEntitiesSubscription.Disposable = Disposable.Empty;

      ViewModels.Clear();

      loadEntitiesSubscription.Disposable =
        Observable.Start(() => Query.ToList(), schedulersFactory.ThreadPool)
          .ObserveOn(DispatcherScheduler)
          .Finally(() => IsLoading = false)
          .Subscribe(models =>
          {
            foreach (var model in models)
            {
              AddViewModel(model);
            }
          }, OnException);
    }

    protected virtual void OnException(Exception error)
    {
    }

    private SerialDisposable whenDataChangesSubscription;

    public void SubscribeToDataChanges()
    {      
      if (disposable.IsDisposed)
        throw new ObjectDisposedException("Object has already been disposed.");
      
      if (whenDataChangesSubscription == null)
        whenDataChangesSubscription = new SerialDisposable();

      var changes = DataChanges;

      var loadingBuffer = changes
        .Buffer(IsLoadingChanged.Where(isLoading => isLoading),
          c => IsLoadingChanged.Where(isLoading => !isLoading))
        .Take(1);

      changes = changes
        .Where(_ => !IsLoading)
        .Merge(loadingBuffer.SelectMany(c => c));

      whenDataChangesSubscription.Disposable = changes
        .Where(c => c.Count > 0)
        .ObserveOn(DispatcherScheduler)
        .Finally(OnDataChangesSubscriptionFinished)
        .Select(CloneEntityChanges)
        .Subscribe(CreateDataChangesObserver());

      LoadEntities();
    }

    private List<EntityChange<TModel>> CloneEntityChanges(IList<EntityChange<TModel>> entityChanges)
    {
      return entityChanges.Select(entityChange =>
      {
        var entity = GetModel(entityChange);

        return new EntityChange<TModel>(entity, entityChange.ChangeType);
      }).ToList();
    }

    protected virtual IObserver<IList<EntityChange<TModel>>> CreateDataChangesObserver()
    {
      return Observer.Create<IList<EntityChange<TModel>>>(OnDataChangesReceived, error => throw error);
    }

    protected virtual void OnDataChangesSubscriptionFinished()
    {
    }

    private void OnDataChangesReceived(IList<EntityChange<TModel>> entityChanges)
    {
      foreach (var entityChange in entityChanges)
      {
        OnDataChangeReceived(entityChange);
      }
    }

    private void OnDataChangeReceived(EntityChange<TModel> entityChange)
    {
      var entity = GetModel(entityChange);

      switch (entityChange.ChangeType)
      {
        case ChangeType.Create:
          OnEntityCreatedNotification(entity);
          break;
        case ChangeType.Update:
          OnEntityUpdatedNotification(entity, UpdateViewModel());
          break;
        case ChangeType.Delete:
          OnEntityDeletedNotification(entity);
          break;
      }
    }

    protected virtual TModel GetModel(EntityChange<TModel> entityChange)
    {
      var entity = entityChange.Entity?.CloneObjectSerializable();

      return entity;
    }

    protected virtual Action<TModel, TViewModel> UpdateViewModel()
    {
      return null;
    }

    protected abstract IEqualityComparer<TModel> Comparer { get; }

    public TViewModel Find(TModel model)
    {
      var viewModel = ViewModels.FirstOrDefault(c => Comparer.Equals(c.Model, model));

      return viewModel;
    }

    protected virtual void OnEntityCreatedNotification(TModel model)
    {
      var viewModel = Find(model);

      if (viewModel == null)
      {
        AddViewModel(model);
      }
    }

    private void AddViewModel(TModel model)
    {
      TViewModel viewModel = CreateViewModel(model);

      ViewModels.Add(viewModel);
    }

    protected virtual bool CanAddMissingEntityOnUpdate(TModel model)
    {
      return model != null;
    }

    protected virtual bool ShouldIgnoreUpdate(TModel currentModel, TModel receivedModel)
    {
      return currentModel.Timestamp >= receivedModel.Timestamp;
    }

    protected virtual void OnEntityUpdatedNotification(TModel model, Action<TModel, TViewModel> update)
    {
      var viewModel = Find(model);

      if (viewModel == null)
      {
        if(CanAddMissingEntityOnUpdate(model))
          AddViewModel(model);

        return;
      }

      if (ShouldIgnoreUpdate(viewModel.Model, model))
        return;

      if (update != null)
      {
        update(model, viewModel);
      }
      else
      {
        OnEntityDeletedNotification(model);

        OnEntityCreatedNotification(model);
      }
    }

    protected virtual bool OnEntityDeletedNotification(TModel model)
    {
      var viewModel = Find(model);

      return ViewModels.Remove(viewModel);
    }

    protected abstract TViewModel CreateViewModel(TModel model);

    protected virtual void OnDispose()
    {
      using (whenDataChangesSubscription)
      using (loadEntitiesSubscription)
      {
      }

      ViewModels.Clear();
    }

    public void Dispose()
    {
      using (disposable)
      { }
    }

    #endregion
  }
}