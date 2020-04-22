using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using StackExchange.Redis;
using UnitTests;

namespace Joker.Redis.Tests.ConnectionMultiplexers
{
  [TestClass]
  public class RedisProviderTests : TestBase<TestableRedisProvider>
  {    
    #region TestInitialize
    
    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      ClassUnderTest = MockingKernel.Get<TestableRedisProvider>();

      ConnectionMultiplexerMock
        .Setup(c => c.GetSubscriber(null))
        .Returns(new Mock<ISubscriber>().Object);
    }
    
    #endregion

    private Mock<IConnectionMultiplexer> ConnectionMultiplexerMock => MockingKernel.GetMock<IConnectionMultiplexer>();

    [TestMethod]
    public async Task ConnectAsync_SubscriberWasCreated()
    {
      //Arrange

      //Act
      await ClassUnderTest.ConnectAsync();

      //Assert
      ConnectionMultiplexerMock
        .Verify(c => c.GetSubscriber(null), Times.Once);
    }

    [TestMethod]
    public async Task ConnectAsync_IsConnected()
    {
      //Arrange
      SetupMultiplexerIsConnected();
      
      //Act
      await ClassUnderTest.ConnectAsync();

      //Assert
      ConnectionMultiplexerMock.Verify();
    }

    private void SetupMultiplexerIsConnected()
    {
      MockingKernel.GetMock<IConnectionMultiplexer>()
        .Setup(c => c.IsConnected)
        .Returns(true)
        .Verifiable();
    }

    #region EventHandlers

    [TestMethod]
    public async Task ConnectAsync_ConnectionRestored()
    {
      //Arrange
      ConnectionMultiplexerMock.SetupAdd(c => c.ConnectionRestored += It.IsAny<EventHandler<ConnectionFailedEventArgs>>());

      //Act
      await ClassUnderTest.ConnectAsync();

      //Assert
      ConnectionMultiplexerMock.VerifyAdd(c => c.ConnectionRestored += It.IsAny<EventHandler<ConnectionFailedEventArgs>>(), Times.Once);
    }

    [TestMethod]
    public async Task Dispose_ConnectionRestored()
    {
      //Arrange
      ConnectionMultiplexerMock.SetupRemove(c => c.ConnectionRestored -= It.IsAny<EventHandler<ConnectionFailedEventArgs>>());
      
      await ClassUnderTest.ConnectAsync();

      //Act
      ClassUnderTest.Dispose();

      //Assert
      ConnectionMultiplexerMock.VerifyRemove(c => c.ConnectionRestored -= It.IsAny<EventHandler<ConnectionFailedEventArgs>>(), Times.Once);
    }

    [TestMethod]
    public async Task ConnectAsync_ConnectionFailed()
    {
      //Arrange
      ConnectionMultiplexerMock.SetupAdd(c => c.ConnectionFailed += It.IsAny<EventHandler<ConnectionFailedEventArgs>>());

      //Act
      await ClassUnderTest.ConnectAsync();

      //Assert
      ConnectionMultiplexerMock.VerifyAdd(c => c.ConnectionFailed += It.IsAny<EventHandler<ConnectionFailedEventArgs>>(), Times.Once);
    }

    [TestMethod]
    public async Task Dispose_ConnectionFailed()
    {
      //Arrange
      ConnectionMultiplexerMock.SetupRemove(c => c.ConnectionFailed -= It.IsAny<EventHandler<ConnectionFailedEventArgs>>());
      
      await ClassUnderTest.ConnectAsync();

      //Act
      ClassUnderTest.Dispose();

      //Assert
      ConnectionMultiplexerMock.VerifyRemove(c => c.ConnectionFailed -= It.IsAny<EventHandler<ConnectionFailedEventArgs>>(), Times.Once);
    }

    #endregion

    [TestMethod]
    public void ConnectAsync_SecondTime_UsesTheSameSubscriber()
    {
      //Arrange
      var t1 = Task.Run(() => ClassUnderTest.CreateSubjectAsync());

      //Act
      var t2 = Task.Run(() => ClassUnderTest.CreateSubjectAsync());

      Task.WaitAll(t1, t2);

      //Assert
      ConnectionMultiplexerMock
        .Verify(c => c.GetSubscriber(null), Times.Once);
    }

    [TestMethod]
    public async Task Dispose()
    {
      //Arrange
      SetupMultiplexerIsConnected();
      await ClassUnderTest.ConnectAsync();
      ClassUnderTest.IsConnected.Should().BeTrue();
      
      //Act
      ClassUnderTest.Dispose();

      //Assert
      ClassUnderTest.IsConnected.Should().BeFalse();
      ConnectionMultiplexerMock
        .Verify(c => c.Dispose(), Times.Once);
    }
  }
}