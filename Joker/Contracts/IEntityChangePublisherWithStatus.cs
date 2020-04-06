using Joker.Notifications;

namespace Joker.Contracts
{
  public interface IEntityChangePublisherWithStatus<TEntity> : IEntityChangePublisher<TEntity>, IPublisher<VersionedTableDependencyStatus>
    where TEntity : IVersion
  {
  }
}