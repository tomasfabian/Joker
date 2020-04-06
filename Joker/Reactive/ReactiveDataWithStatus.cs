using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Joker.Contracts;
using Joker.Notifications;

namespace Joker.Reactive
{
  public class ReactiveDataWithStatus<TModel> : ReactiveData<TModel>, IReactiveDataWithStatus<TModel>, IEntityChangePublisherWithStatus<TModel>
    where TModel : IVersion
  {    
    private readonly ISubject<VersionedTableDependencyStatus> whenStatusChanges = new ReplaySubject<VersionedTableDependencyStatus>(1);

    public IObservable<VersionedTableDependencyStatus> WhenStatusChanges => whenStatusChanges.AsObservable();

    public void Publish(VersionedTableDependencyStatus status)
    {
      whenStatusChanges.OnNext(status);
    }
  }
}