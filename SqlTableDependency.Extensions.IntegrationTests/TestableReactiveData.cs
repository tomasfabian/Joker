using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Joker.Contracts;
using Joker.Enums;
using Joker.Notifications;
using Joker.Reactive;

namespace SqlTableDependency.Extensions.IntegrationTests
{
  public class TestableReactiveData<TModel> : IReactiveDataWithStatus<TModel>, IEntityChangePublisherWithStatus<TModel>
    where TModel : IVersion
  {
    private readonly ISubject<EntityChange<TModel>> whenDataChanges = new ReplaySubject<EntityChange<TModel>>(1);

    public IObservable<EntityChange<TModel>> WhenDataChanges => whenDataChanges.AsObservable();

    public void Publish(EntityChange<TModel> entityChange)
    {
      whenDataChanges.OnNext(entityChange);
    }

    public static readonly ReactiveDataWithStatus<TModel> Instance = new ReactiveDataWithStatus<TModel>();

    private readonly ISubject<VersionedTableDependencyStatus> whenStatusChanges = new ReplaySubject<VersionedTableDependencyStatus>(1);

    public IObservable<VersionedTableDependencyStatus> WhenStatusChanges => whenStatusChanges.AsObservable();

    public void Publish(VersionedTableDependencyStatus status)
    {
      whenStatusChanges.OnNext(status);
    }
  }
}