using Joker.Notifications;

namespace Joker.Contracts
{
  public interface IPublisherWithStatus<TEntity> : IPublisher<TEntity>
    where TEntity : IVersion
  {
    void PublishStatus(VersionedTableDependencyStatus status);
  }
}