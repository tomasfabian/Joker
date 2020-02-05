using System;
using SqlTableDependency.Extensions.Providers.Sql;
using TableDependency.SqlClient.Base.Abstracts;

namespace SqlTableDependency.Extensions
{
  public interface ISqlTableDependencyWithReconnection<TEntity> : ITableDependency<TEntity> 
    where TEntity : class, new()
  {

  }

  public interface ISqlTableDependencyWithReconnection : IDisposable
  {
    ISqlConnectionProvider SqlConnectionProvider { get; }

    void Start(int timeOut = 120, int watchDogTimeOut = 180);

    void Stop();
  }
}