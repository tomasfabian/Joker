using System.Threading.Tasks;
using StackExchange.Redis;

namespace SqlTableDependency.Extensions.Redis.ConnectionMultiplexers
{
  public interface IRedisPublisher
  {
    Task PublishAsync(RedisChannel redisChannel, RedisValue redisValue, CommandFlags commandFlags = CommandFlags.None);
  }
}