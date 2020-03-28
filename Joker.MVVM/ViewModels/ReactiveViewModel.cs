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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Joker.Disposables;
using Joker.MVVM.Contracts;
using Joker.MVVM.Enums;

namespace Joker.MVVM.ViewModels
{
  public abstract class ReactiveViewModel<TModel, TViewModel> : ViewModelsList<TViewModel>, IDisposable
    where TModel : IVersion
    where TViewModel : ViewModel<TModel>, IVersion
  {
    #region Fields

    private readonly IReactive<TModel> reactive;
    private readonly IDisposable disposable;

    #endregion

    #region Constructors

    protected ReactiveViewModel(IReactive<TModel> reactive)
    {
      this.reactive = reactive ?? throw new ArgumentNullException(nameof(reactive));

      disposable = DisposableObject.Create(() =>
                                           {
                                             using (whenDataChangesSubscription)
                                             {
                                             }
                                           });
    }

    #endregion

    #region Properties

    protected abstract IScheduler DispatcherScheduler { get; }

    #endregion

    #region Methods

    private IDisposable whenDataChangesSubscription;

    public void SubscribeToDataChanges()
    {
      using (whenDataChangesSubscription)
      { }

      whenDataChangesSubscription = reactive.WhenDataChanges
        .ObserveOn(DispatcherScheduler)
        .Subscribe(OnDataChangeReceived);
    }

    private void OnDataChangeReceived(EntityChange<TModel> entityChange)
    {
      var entity = entityChange.Entity;

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
        viewModel = CreateViewModel(model);

        ViewModels.Add(viewModel);
      }
    }

    protected virtual void OnEntityUpdated(TModel model, Action<TModel, TViewModel> update)
    {
      if (update != null)
      {
        var viewModel = Find(model);

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

    public void Dispose()
    {
      using (disposable)
      { }
    }

    #endregion
  }
}