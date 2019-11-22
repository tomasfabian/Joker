using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace SqlTableDependency.Extensions.Redis.ConnectionMultiplexers
{
  public class RedisSubscriber : RedisProvider, IRedisSubscriber
  {
    private readonly string url;

    public RedisSubscriber(string url)
    {
      this.url = url ?? throw new ArgumentNullException(nameof(url));
    }

    public async Task Subscribe(Action<ChannelMessage> onMessage, RedisChannel redisChannel, CommandFlags commandFlags = CommandFlags.None)
    {
      Subject = await CreateSubject(url);

      var channel = await Subject.SubscribeAsync(redisChannel);
      
      channel.OnMessage(onMessage);
    }
  }
}