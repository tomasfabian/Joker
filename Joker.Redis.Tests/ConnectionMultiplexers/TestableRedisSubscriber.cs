using System;
using System.Threading.Tasks;
using Joker.Redis.ConnectionMultiplexers;
using StackExchange.Redis;

namespace Joker.Redis.Tests.ConnectionMultiplexers
{
  public class TestableRedisSubscriber : RedisSubscriber
  {
    private readonly IConnectionMultiplexer connectionMultiplexer;

    public TestableRedisSubscriber(string url, IConnectionMultiplexer connectionMultiplexer)
      : base(url)
    {
      this.connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
    }

    internal override Task<IConnectionMultiplexer> CreateConnectionMultiplexer(string url)
    {
      return Task.FromResult(connectionMultiplexer);
    }
  }
}