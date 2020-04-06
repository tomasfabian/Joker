using Joker.Enums;

namespace Joker.Contracts
{
  public interface IEntityChangePublisher<TEntity> : IPublisher<EntityChange<TEntity>>
    where TEntity : IVersion
  {
  }
}