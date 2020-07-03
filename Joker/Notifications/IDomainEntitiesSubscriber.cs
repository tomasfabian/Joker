using System.Threading.Tasks;

namespace Joker.Notifications
{
  public interface IDomainEntitiesSubscriber
  {
    Task Subscribe();
  }
}