using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using Joker.Comparators;
using Joker.MVVM.Contracts;
using Joker.MVVM.Enums;
using Joker.MVVM.ViewModels;
using UnitTests.Schedulers;

namespace Joker.MVVM.Tests.Helpers
{
  public class ReactiveListTestViewModel : ReactiveListViewModel<TestModel, TestViewModel>
  {
    private readonly ReactiveTestSchedulersFactory schedulersFactory;

    public ReactiveListTestViewModel(IReactive<TestModel> reactive, ReactiveTestSchedulersFactory schedulersFactory) 
      : base(reactive, schedulersFactory)
    {
      this.schedulersFactory = schedulersFactory ?? throw new ArgumentNullException(nameof(schedulersFactory));

      Comparer = new GenericEqualityComparer<TestModel>((x, y) => x.Id == y.Id);
    }

    protected override IScheduler DispatcherScheduler => schedulersFactory.Dispatcher;
    
    protected override IEnumerable<TestModel> Query => new[]
    {
      new TestModel
      {
        Id = 1,
        Timestamp = new DateTime(2019, 3, 27, 16, 0, 0),
        Name = "Model 1"
      }
    };

    protected override IEqualityComparer<TestModel> Comparer { get; }

    protected override TestViewModel CreateViewModel(TestModel model)
    {
      return new TestViewModel(model);
    }

    public bool CreateUpdateViewModelAction { get; set; }

    protected override Action<TestModel, TestViewModel> UpdateViewModel()
    {
      if (CreateUpdateViewModelAction)
        return (model, viewModel) => viewModel.UpdateFrom(model);

      return null;
    }    
    
    protected override TestModel GetModel(EntityChange<TestModel> entityChange)
    {
      var entity = entityChange.Entity?.Clone();

      return entity;
    }
  }
}