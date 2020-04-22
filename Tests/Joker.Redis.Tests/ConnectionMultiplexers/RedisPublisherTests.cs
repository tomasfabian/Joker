using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StackExchange.Redis;

namespace Joker.Redis.Tests.ConnectionMultiplexers
{
  [TestClass]
  public class RedisPublisherTests : RedisProviderBaseTests<TestableRedisPublisher>
  {    
    [TestMethod]
    public async Task SetStringAsync_MultiplexerIsNotConnected()
    {
      //Arrange

      //Act
      var result = await ClassUnderTest.SetStringAsync("key", "value");

      //Assert
      result.Should().BeFalse();
      RedisDatabaseMock.Verify(c => c.StringSetAsync(It.IsAny<KeyValuePair<RedisKey, RedisValue>[]>(), When.Always, CommandFlags.None), Times.Never);
    }

    [TestMethod]
    public async Task SetStringAsync_MultiplexerIsConnected()
    {
      //Arrange
      SetupMultiplexerIsConnected();

      RedisDatabaseMock.Setup(c => c.StringSetAsync(It.IsAny<KeyValuePair<RedisKey, RedisValue>[]>(), When.Always, CommandFlags.None))
        .ReturnsAsync(true)
        .Verifiable();

      //Act
      var result = await ClassUnderTest.SetStringAsync("key", "value");

      //Assert
      result.Should().BeTrue();
      ConnectionMultiplexerMock.Verify();
    }
    
    [TestMethod]
    public async Task PublishAsync_MultiplexerIsNotConnected()
    {
      //Arrange

      //Act
      var result = await ClassUnderTest.PublishAsync("channelName", "value");

      //Assert
      result.Should().Be(-1);
    }
    
    [TestMethod]
    public async Task PublishAsync_MultiplexerIsConnected()
    {
      //Arrange
      SetupMultiplexerIsConnected();

      SubscriberMock.Setup(c => c.PublishAsync(It.IsAny<RedisChannel>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
        .ReturnsAsync(0)
        .Verifiable();

      //Act
      var result = await ClassUnderTest.PublishAsync("channelName", "value");

      //Assert
      result.Should().Be(0);
      SubscriberMock.Verify();
    }
    
    [TestMethod]
    public async Task SetStringRetryCount_StringSetAsyncWasCalledRetryCountTimes()
    {
      //Arrange
      SetupMultiplexerIsConnected();
      ClassUnderTest.SetStringRetryCount = 1;

      RedisDatabaseMock.Setup(c => c.StringSetAsync(It.IsAny<KeyValuePair<RedisKey, RedisValue>[]>(), When.Always, CommandFlags.None))
        .ThrowsAsync(new Exception())
        .Verifiable();

      //Act
      var result = await ClassUnderTest.SetStringAsync("key", "value");

      //Assert
      result.Should().BeFalse();        
      RedisDatabaseMock.Verify(c => c.StringSetAsync(It.IsAny<KeyValuePair<RedisKey, RedisValue>[]>(), When.Always, CommandFlags.None), Times.Exactly(ClassUnderTest.SetStringRetryCount + 1));
    }
  }
}