using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace SqlTableDependency.Extensions.Redis.ConnectionMultiplexers
{
  public interface IRedisSubscriber : IRedisProvider
  {
    Task Subscribe(Action<ChannelMessage> onMessage, RedisChannel redisChannel, CommandFlags commandFlags = CommandFlags.None);
  }
}