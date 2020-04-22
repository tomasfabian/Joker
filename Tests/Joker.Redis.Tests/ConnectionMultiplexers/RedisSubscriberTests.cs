using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StackExchange.Redis;

namespace Joker.Redis.Tests.ConnectionMultiplexers
{
  [TestClass]
  public class RedisSubscriberTests : RedisProviderBaseTests<TestableRedisSubscriber>
  {
    [TestMethod]
    public async Task GetStringAsync_MultiplexerIsNotConnected()
    {
      //Arrange

      //Act
      var result = await ClassUnderTest.GetStringAsync("key");

      //Assert
      result.Should().BeNull();
      RedisDatabaseMock.Verify(c => c.StringGetAsync("key", CommandFlags.None), Times.Never);
    }

    [TestMethod]
    public async Task GetStringAsync_MultiplexerIsConnected()
    {
      //Arrange
      SetupMultiplexerIsConnected();
      string value = "value";
      RedisDatabaseMock.Setup(c => c.StringGetAsync("key", CommandFlags.None))
        .ReturnsAsync(value)
        .Verifiable();

      //Act
      var result = await ClassUnderTest.GetStringAsync("key");

      //Assert
      result.Should().Be(value);
      RedisDatabaseMock.Verify();
    }
    
    [TestMethod]
    public async Task GetStringRetryCount_StringSetAsyncWasCalledRetryCountTimes()
    {
      //Arrange
      SetupMultiplexerIsConnected();
      ClassUnderTest.GetStringRetryCount = 1;
      
      RedisDatabaseMock.Setup(c => c.StringGetAsync("key", CommandFlags.None))
        .ThrowsAsync(new Exception())
        .Verifiable();

      //Act
      var result = await ClassUnderTest.GetStringAsync("key");

      //Assert
      result.Should().BeNull();        
      RedisDatabaseMock.Verify(c => c.StringGetAsync("key", CommandFlags.None), Times.Exactly(ClassUnderTest.GetStringRetryCount + 1));
    }
    
    [TestMethod]
    public async Task Subscribe()
    {
      //Arrange
      SubscriberMock.Setup(c => c.SubscribeAsync(It.IsAny<RedisChannel>(), It.IsAny<CommandFlags>()))
        .ReturnsAsync(default(ChannelMessageQueue))
        .Verifiable();

      //Act
      await ClassUnderTest.Subscribe(cm => {}, "ChannelName");

      //Assert
      SubscriberMock.Verify();
    }

    [TestMethod]
    public void Unsubscribe()
    {
      //Arrange
      

      //Act
      ClassUnderTest.Unsubscribe("ChannelName");

      //Assert
      SubscriberMock.Verify(c => c.UnsubscribeAsync(It.IsAny<RedisChannel>(), It.IsAny<Action<RedisChannel, RedisValue>>(), It.IsAny<CommandFlags>()), Times.Never);
    }

    [TestMethod]
    public async Task Connected_Unsubscribe()
    {
      //Arrange
      await ClassUnderTest.Subscribe(cm => { }, "ChannelName");

      //Act
      ClassUnderTest.Unsubscribe("ChannelName");

      //Assert
      SubscriberMock.Verify(c => c.UnsubscribeAsync(It.IsAny<RedisChannel>(), It.IsAny<Action<RedisChannel, RedisValue>>(), It.IsAny<CommandFlags>()), Times.Once);
    }
  }
}