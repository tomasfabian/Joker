namespace Joker.Contracts
{
  public interface IPublisher<in TNotification>
  {
    void Publish(TNotification notification);
  }
}