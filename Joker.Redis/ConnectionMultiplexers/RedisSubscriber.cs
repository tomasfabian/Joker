using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Joker.Redis.ConnectionMultiplexers
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
      await CreateSubject(url);

      var channel = await Subject.SubscribeAsync(redisChannel);
      
      channel.OnMessage(onMessage);
    }

    public async Task<RedisValue> GetStringAsync(string key)
    {
      await CreateSubject(url);
      
      IDatabase redisDatabase = GetDatabase();

      return await redisDatabase.StringGetAsync(key);
    }

    public void Unsubscribe(RedisChannel channel, Action<RedisChannel, RedisValue> handler = null, CommandFlags flags = CommandFlags.None)
    {
      Subject?.Unsubscribe(channel, handler, flags);
    }
  }
}