using System.Threading.Tasks;
using StackExchange.Redis;

namespace Joker.Redis.ConnectionMultiplexers
{
  public interface IRedisPublisher
  {
    Task PublishAsync(RedisChannel redisChannel, RedisValue redisValue, CommandFlags commandFlags = CommandFlags.None);
  }
}