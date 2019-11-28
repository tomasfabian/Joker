using System;

namespace SqlTableDependency.Extensions.Redis.SqlTableDependency
{
  public interface ISqlTableDependencyRedisProvider<TEntity> : IDisposable
    where TEntity : class, new()
  {
    SqlTableDependencyRedisProvider<TEntity> StartPublishing();
  }
}