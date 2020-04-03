using System;
using Joker.Notifications;

namespace Joker.Contracts
{
  public interface IReactiveDataWithStatus<TEntity> : IReactiveData<TEntity>
    where TEntity : IVersion
  {
    IObservable<VersionedTableDependencyStatus> WhenStatusChanges { get; }
  }
}