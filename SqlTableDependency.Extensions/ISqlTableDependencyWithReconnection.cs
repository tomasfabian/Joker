using System;
using TableDependency.SqlClient.Base.Abstracts;

namespace SqlTableDependency.Extensions
{
  public interface ISqlTableDependencyWithReconnection<TEntity> : ITableDependency<TEntity> 
    where TEntity : class, new()
  {

  }
}