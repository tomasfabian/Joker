using System;
using Joker.Contracts;

namespace Joker.Enums
{
  public class EntityChange<TEntity>
    where TEntity : IVersion
  {
    public EntityChange(TEntity entity, ChangeType changeType)
    {
      if (entity == null)
        throw new ArgumentNullException(nameof(entity));
        
      Entity = entity;
      ChangeType = changeType;
    }

    public TEntity Entity { get; }
    public ChangeType ChangeType { get; }
  }
}