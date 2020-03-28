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
    where TViewModel : ViewModel<TModel>, IVersion
  {
    #region Fields

    private readonly IReactiveData<TModel> reactive;
    private readonly ISchedulersFactory schedulersFactory;
    private IDisposable disposable;

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

    #endregion

    #region Methods

    private void Init()
    {
      disposable = DisposableObject.Create(() =>
      {
        OnDispose();

        using (whenDataChangesSubscription)
        using (loadEntitiesSubscription)
        {
        }
      });
    }

    private SerialDisposable loadEntitiesSubscription;

    private void LoadEntities()
    {
      if (loadEntitiesSubscription == null)
        loadEntitiesSubscription = new SerialDisposable();

      IsLoading = true;

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

    protected virtual IObservable<IList<EntityChange<TModel>>> DataChanges => reactive.WhenDataChanges
      .Buffer(TimeSpan.FromMilliseconds(250), 100, schedulersFactory.TaskPool);

    private SerialDisposable whenDataChangesSubscription;

    public void SubscribeToDataChanges()
    {
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
        .Subscribe(OnDataChangesReceived);

      LoadEntities();
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
          OnEntityCreated(entity);
          break;
        case ChangeType.Update:
          OnEntityUpdated(entity, UpdateViewModel());
          break;
        case ChangeType.Delete:
          OnEntityDeleted(entity);
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

    protected virtual void OnEntityCreated(TModel model)
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

    protected virtual void OnEntityUpdated(TModel model, Action<TModel, TViewModel> update)
    {
      var viewModel = Find(model);

      if (viewModel == null)
      {
        AddViewModel(model);

        return;
      }

      if (viewModel.Timestamp >= model.Timestamp)
        return;

      if (update != null)
      {
        update(model, viewModel);
      }
      else
      {
        OnEntityDeleted(model);

        OnEntityCreated(model);
      }
    }

    protected virtual bool OnEntityDeleted(TModel model)
    {
      var viewModel = Find(model);

      return ViewModels.Remove(viewModel);
    }

    protected abstract TViewModel CreateViewModel(TModel model);

    protected virtual void OnDispose()
    {
    }

    public void Dispose()
    {
      using (disposable)
      { }
    }

    #endregion
  }
}