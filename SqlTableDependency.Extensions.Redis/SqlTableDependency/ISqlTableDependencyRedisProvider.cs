using System;

namespace Joker.Redis.SqlTableDependency
{
  public interface ISqlTableDependencyRedisProvider<TEntity> : IDisposable
    where TEntity : class, new()
  {
    SqlTableDependencyRedisProvider<TEntity> StartPublishing();
  }
}