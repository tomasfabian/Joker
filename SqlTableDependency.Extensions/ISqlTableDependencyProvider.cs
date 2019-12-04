using System;
using SqlTableDependency.Extensions.Notifications;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;

namespace SqlTableDependency.Extensions
{
  public interface ISqlTableDependencyProvider
  {
    void SubscribeToEntityChanges();
  }

  public interface ISqlTableDependencyProvider<TEntity> : ISqlTableDependencyProvider
    where TEntity : class, new()
  {
    IObservable<RecordChangedNotification<TEntity>> WhenEntityRecordChanges { get; }
    IObservable<TableDependencyStatus> WhenStatusChanges { get; }
  }
}