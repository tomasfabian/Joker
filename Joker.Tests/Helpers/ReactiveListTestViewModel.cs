using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using Joker.Contracts;
using Joker.Enums;
using Joker.MVVM.ViewModels;
using UnitTests.Schedulers;

namespace Joker.MVVM.Tests.Helpers
{
  public class ReactiveListTestViewModel : ReactiveListViewModel<TestModel, TestViewModel>
  {
    private readonly ReactiveTestSchedulersFactory schedulersFactory;

    public ReactiveListTestViewModel(IReactiveData<TestModel> reactive, ReactiveTestSchedulersFactory schedulersFactory) 
      : base(reactive, schedulersFactory)
    {
      this.schedulersFactory = schedulersFactory ?? throw new ArgumentNullException(nameof(schedulersFactory));
    }

    protected override int DataChangesBufferCount => 3;

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

    protected override IComparable GetId(TestModel model)
    {
      return model.Id;
    }

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

    public bool CanAddMissingEntityOnUpdateOverride { get; set; } = true;

    protected override bool CanAddMissingEntityOnUpdate(TestModel model)
    {
      return CanAddMissingEntityOnUpdateOverride;
    }

    protected override TestModel GetModel(EntityChange<TestModel> entityChange)
    {
      var entity = entityChange.Entity?.Clone();

      return entity;
    }
  }
}