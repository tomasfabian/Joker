using System.Threading.Tasks;
using StackExchange.Redis;

namespace Joker.Redis.ConnectionMultiplexers
{
  public interface IRedisPublisher : IRedisProvider
  {
    Task<long> PublishAsync(RedisChannel redisChannel, RedisValue redisValue, CommandFlags commandFlags = CommandFlags.None);
    Task<bool> SetStringAsync(string key, string value);
    int SetStringRetryCount { get; set; }
  }
}