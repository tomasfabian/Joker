using System;
using System.Threading.Tasks;
using Joker.Redis.ConnectionMultiplexers;
using StackExchange.Redis;

namespace Joker.Redis.Tests.ConnectionMultiplexers
{
  public class TestableRedisProvider : RedisProvider
  {
    private readonly IConnectionMultiplexer connectionMultiplexer;

    public TestableRedisProvider(IConnectionMultiplexer connectionMultiplexer)
    {
      this.connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
    }

    public async Task ConnectAsync()
    {
      await CreateSubject("localhost");
    }    
    
    public Task<ISubscriber> CreateSubjectAsync()
    {
      return CreateSubject("localhost");
    }

    internal override Task<IConnectionMultiplexer> CreateConnectionMultiplexer(string url)
    {
      return Task.FromResult(connectionMultiplexer);
    }
  }
}