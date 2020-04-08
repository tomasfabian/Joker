using System;
using System.Collections.Generic;
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
    
    public async Task<long> PublishAsync(RedisChannel redisChannel, RedisValue redisValue, CommandFlags commandFlags = CommandFlags.None)
    {
      await CreateSubject(url);

      if (!IsConnected)
        return -1;

      try
      {
        return await Subject.PublishAsync(redisChannel, redisValue, commandFlags);
      }
      catch (Exception e)
      {
        OnError(e);

        return -1;
      }
    }    

    private long Publish(RedisChannel redisChannel, RedisValue redisValue, CommandFlags commandFlags = CommandFlags.None)
    {
      var subject = CreateSubject(url).Result;

      return Subject.Publish(redisChannel, redisValue, commandFlags);
    }

    public int SetStringRetryCount { get; set; }

    public async Task<bool> SetStringAsync(string key, string value)
    {
      await CreateSubject(url);

      return await TrySetStringAsync(key, value, SetStringRetryCount);
    }

    private async Task<bool> TrySetStringAsync(string key, string value, int retryCount)
    {
      try
      {
        var redisDatabase = GetDatabase();

        if (redisDatabase == null || !redisDatabase.Multiplexer.IsConnected ||
            !redisDatabase.IsConnected(default(RedisKey)))
        {
          return false;
        }

        var setStringResult = await redisDatabase.StringSetAsync(new[]
        {
          new KeyValuePair<RedisKey, RedisValue>(key, value)
        });

        return setStringResult;
      }
      catch (Exception e)
      {
        await Task.Delay(TimeSpan.FromMilliseconds(25));

        if (retryCount-- > 0)
          return await TrySetStringAsync(key, value, retryCount);

        OnError(e);

        return false;
      }
    }

    protected virtual void OnError(Exception error)
    {
    }
  }
}