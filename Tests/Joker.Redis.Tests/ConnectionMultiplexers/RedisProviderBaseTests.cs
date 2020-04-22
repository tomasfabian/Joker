using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using Ninject.Parameters;
using StackExchange.Redis;
using UnitTests;

namespace Joker.Redis.Tests.ConnectionMultiplexers
{
  [TestClass]
  public abstract class RedisProviderBaseTests<TRedisProvider> : TestBase<TRedisProvider>
  {
    #region TestInitialize

    protected Mock<IConnectionMultiplexer> ConnectionMultiplexerMock => MockingKernel.GetMock<IConnectionMultiplexer>();

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();
      
      ClassUnderTest = MockingKernel.Get<TRedisProvider>(new ConstructorArgument("url", "localhost"));

      SetupMocks();
    }

    protected Mock<IDatabase> RedisDatabaseMock;
    protected Mock<ISubscriber> SubscriberMock;

    private void SetupMocks()
    {
      SubscriberMock = new Mock<ISubscriber>();
      ConnectionMultiplexerMock
        .Setup(c => c.GetSubscriber(null))
        .Returns(SubscriberMock.Object);

      RedisDatabaseMock = new Mock<IDatabase>();
      RedisDatabaseMock.Setup(c => c.Multiplexer)
        .Returns(ConnectionMultiplexerMock.Object);
      RedisDatabaseMock.Setup(c => c.IsConnected(default, CommandFlags.None))
        .Returns(true);

      ConnectionMultiplexerMock
        .Setup(c => c.GetDatabase(-1, null))
        .Returns(RedisDatabaseMock.Object);
    }
    
    protected void SetupMultiplexerIsConnected()
    {
      ConnectionMultiplexerMock
        .Setup(c => c.IsConnected)
        .Returns(true)
        .Verifiable();
    }

    #endregion
  }
}