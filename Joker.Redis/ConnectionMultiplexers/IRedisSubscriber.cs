using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Joker.Redis.ConnectionMultiplexers
{
  public interface IRedisSubscriber : IRedisProvider
  {
    Task Subscribe(Action<ChannelMessage> onMessage, RedisChannel redisChannel, CommandFlags commandFlags = CommandFlags.None);
    void Unsubscribe(RedisChannel channel, Action<RedisChannel, RedisValue> handler = null, CommandFlags flags = CommandFlags.None);
    Task<string> GetStringAsync(string key);
  }
}