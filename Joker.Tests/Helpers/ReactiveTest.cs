using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Joker.Contracts;
using Joker.Enums;

namespace Joker.MVVM.Tests.Helpers
{
  public class ReactiveTest : IReactive<TestModel>
  {
    private readonly ISubject<EntityChange<TestModel>> whenDataChanges = new Subject<EntityChange<TestModel>>();

    public IObservable<EntityChange<TestModel>> WhenDataChanges => whenDataChanges.AsObservable();

    public void Publish(EntityChange<TestModel> entityChange)
    {
      whenDataChanges.OnNext(entityChange);
    }
  }
}