using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Joker.Redis.ConnectionMultiplexers
{
  public class RedisPublisher : RedisProvider, IRedisPublisher
  {
    private readonly string url;

    public RedisPublisher(string url)
    {
      this.url = url ?? throw new ArgumentNullException(nameof(url));
    }
    
    public async Task PublishAsync(RedisChannel redisChannel, RedisValue redisValue, CommandFlags commandFlags = CommandFlags.None)
    {
      await CreateSubject(url); 
      
      await Subject.PublishAsync(redisChannel, redisValue, commandFlags);
    }
  }
}