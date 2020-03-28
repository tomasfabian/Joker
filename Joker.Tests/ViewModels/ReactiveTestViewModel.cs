using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using Joker.Comparators;
using Joker.MVVM.Contracts;
using Joker.MVVM.ViewModels;
using UnitTests.Schedulers;

namespace Joker.MVVM.Tests.ViewModels
{
  public class ReactiveTestViewModel : ReactiveViewModel<TestModel, TestViewModel>
  {
    private readonly ReactiveTestSchedulersFactory schedulersFactory;

    public ReactiveTestViewModel(IReactive<TestModel> reactive, ReactiveTestSchedulersFactory schedulersFactory) : base(reactive)
    {
      this.schedulersFactory = schedulersFactory ?? throw new ArgumentNullException(nameof(schedulersFactory));

      Comparer = new GenericEqualityComparer<TestModel>((x, y) => x.Id == y.Id);
    }

    protected override IScheduler DispatcherScheduler => schedulersFactory.Dispatcher;
    protected override IEqualityComparer<TestModel> Comparer { get; }

    protected override TestViewModel CreateViewModel(TestModel model)
    {
      return new TestViewModel(model);
    }
  }
}