using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using Joker.Contracts;
using Joker.MVVM.ViewModels.Domain;
using UnitTests.Schedulers;

namespace Joker.MVVM.Tests.Helpers
{
  public class NotSerializableTestModelViewModels : ReactiveListDomainEntityViewModel<NotSerializableTestModel, NotSerializableTestModelViewModel>
  {
    private readonly ReactiveTestSchedulersFactory schedulersFactory;

    public NotSerializableTestModelViewModels(IReactiveData<NotSerializableTestModel> reactive, ReactiveTestSchedulersFactory schedulersFactory) 
      : base(reactive, schedulersFactory)
    {
      this.schedulersFactory = schedulersFactory ?? throw new ArgumentNullException(nameof(schedulersFactory));
    }
    
    protected override IScheduler DispatcherScheduler => schedulersFactory.Dispatcher;

    protected override IEnumerable<NotSerializableTestModel> Query => new[]
    {
      new NotSerializableTestModel
      {
        Id = 1,
        Timestamp = new DateTime(2019, 3, 27, 16, 0, 0),
        Name = "Model 1"
      }
    };

    protected override IComparable GetId(NotSerializableTestModel model)
    {
      return model.Id;
    }

    protected override NotSerializableTestModelViewModel CreateViewModel(NotSerializableTestModel model)
    {
      return new NotSerializableTestModelViewModel(model);
    }
  }
}