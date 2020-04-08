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
      
      channel?.OnMessage(onMessage);
    }

    public async Task<string> GetStringAsync(string key)
    {
      await CreateSubject(url);

      return await TryGetStringAsync(key, GetStringRetryCount);
    }

    public int GetStringRetryCount { get; set; } = 0;

    private async Task<string> TryGetStringAsync(string key, int retryCount)
    {
      try
      {
        var redisDatabase = GetDatabase();

        if (redisDatabase == null || !redisDatabase.Multiplexer.IsConnected)
          return null;

        var value = await redisDatabase.StringGetAsync(key);

        return value;
      }
      catch (Exception e)
      {
        await Task.Delay(TimeSpan.FromMilliseconds(25));

        if (retryCount-- > 0)
          return await TryGetStringAsync(key, retryCount);

        OnError(e);

        return null;
      }
    }

    protected virtual void OnError(Exception error)
    {
    }

    public void Unsubscribe(RedisChannel channel, Action<RedisChannel, RedisValue> handler = null, CommandFlags flags = CommandFlags.None)
    {
      Subject?.UnsubscribeAsync(channel, handler, flags);
    }
  }
}