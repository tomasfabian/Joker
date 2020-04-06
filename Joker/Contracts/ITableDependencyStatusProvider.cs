using System;
using Joker.Notifications;

namespace Joker.Contracts
{
  public interface ITableDependencyStatusProvider
  {
    IObservable<VersionedTableDependencyStatus> WhenStatusChanges { get; }
  }
}