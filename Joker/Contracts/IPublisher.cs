using Joker.Enums;

namespace Joker.Contracts
{
  public interface IPublisher<TEntity>
    where TEntity : IVersion
  {
    void Publish(EntityChange<TEntity> entityChange);
  }
}