using System.Threading.Tasks;

namespace Joker.Redis.Notifications
{
  public interface IDomainEntitiesSubscriber
  {
    Task Subscribe();
  }
}