using System;
using SqlTableDependency.Extensions.Notifications;
using TableDependency.SqlClient.Base.Enums;

namespace SqlTableDependency.Extensions
{
  public interface ISqlTableDependencyProvider : IDisposable
  {
    IObservable<TableDependencyStatus> WhenStatusChanges { get; }

    void SubscribeToEntityChanges();
  }

  public interface ISqlTableDependencyProvider<TEntity> : ISqlTableDependencyProvider
    where TEntity : class, new()
  {
    IObservable<RecordChangedNotification<TEntity>> WhenEntityRecordChanges { get; }
  }
}