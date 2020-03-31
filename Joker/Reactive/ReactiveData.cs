using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Joker.Contracts;
using Joker.Enums;

namespace Joker.Reactive
{
  public class ReactiveData<TModel> : IReactiveData<TModel>, IPublisher<TModel>
    where TModel : IVersion
  {
    private readonly ISubject<EntityChange<TModel>> whenDataChanges = new Subject<EntityChange<TModel>>();

    public IObservable<EntityChange<TModel>> WhenDataChanges => whenDataChanges.AsObservable();

    public void Publish(EntityChange<TModel> entityChange)
    {
      whenDataChanges.OnNext(entityChange);
    }
  }
}