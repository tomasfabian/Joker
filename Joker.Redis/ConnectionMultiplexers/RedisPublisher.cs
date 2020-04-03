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

      return await Subject.PublishAsync(redisChannel, redisValue, commandFlags);
    }    

    private long Publish(RedisChannel redisChannel, RedisValue redisValue, CommandFlags commandFlags = CommandFlags.None)
    {
      var subject = CreateSubject(url).Result;

      return Subject.Publish(redisChannel, redisValue, commandFlags);
    }

    public async Task<bool> SetStringAsync(string key, string value)
    {
      await CreateSubject(url);
      
      IDatabase redisDatabase = GetDatabase();

      var setStringResult = await redisDatabase.StringSetAsync(new[]
      {
        new KeyValuePair<RedisKey, RedisValue>(key, value)
      });

      return setStringResult;
    }
  }
}