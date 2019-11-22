using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace SqlTableDependency.Extensions.Redis.ConnectionMultiplexers
{
  public class RedisPublisher : RedisProvider, IRedisPublisher
  {
    private readonly string url;

    public RedisPublisher(string url)
    {
      this.url = url ?? throw new ArgumentNullException(nameof(url));
    }

    public async Task Publish(RedisChannel redisChannel, RedisValue redisValue, CommandFlags commandFlags = CommandFlags.None)
    {
      Subject = await CreateSubject(url);

      await Subject.PublishAsync(redisChannel, redisValue, commandFlags);
    }
  }
}