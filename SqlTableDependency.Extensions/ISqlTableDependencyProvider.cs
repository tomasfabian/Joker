using System;
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
    IObservable<RecordChangedEventArgs<TEntity>> WhenEntityRecordChanges { get; }
  }
}