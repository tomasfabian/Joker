using System;
using System.Threading.Tasks;
using FluentAssertions;
using Joker.Contracts;
using Joker.Enums;
using Joker.Notifications;
using Joker.Redis.ConnectionMultiplexers;
using Joker.Redis.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using StackExchange.Redis;
using ChangeType = TableDependency.SqlClient.Base.Enums.ChangeType;
using UnitTests;
using TableDependencyStatuses = Joker.Notifications.VersionedTableDependencyStatus.TableDependencyStatuses;

namespace Joker.Redis.Tests.Notifications
{
  [TestClass]
  public class DomainEntitiesSubscriberTests : TestBase<TestableDomainEntitiesSubscriber>
  {
    #region TestInitialize

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      MockingKernel.GetMock<IRedisSubscriber>()
        .Setup(c => c.Subscribe(It.IsAny<Action<ChannelMessage>>(), It.IsAny<RedisChannel>(), It.IsAny<CommandFlags>()))
        .Returns(Task.CompletedTask);

      ClassUnderTest = MockingKernel.Get<TestableDomainEntitiesSubscriber>();
    }

    #endregion

    #region Tests

    #region EntityChange

    [TestMethod]
    public async Task Subscribe_SubscribedToRedisChannel()
    {
      //Arrange
      
      //Act
      await ClassUnderTest.Subscribe();

      //Assert
      MockingKernel.GetMock<IRedisSubscriber>()
        .Verify(c => c.Subscribe(It.IsAny<Action<ChannelMessage>>(), It.Is<RedisChannel>(d => d == $"{typeof(TestModel).Name}-Changes"), It.IsAny<CommandFlags>()), Times.Once);
    }

    [TestMethod]
    public async Task Subscribe_OnMessageWasSet()
    {
      //Arrange
      Action<ChannelMessage> onMessage = null;

      MockingKernel.GetMock<IRedisSubscriber>()
        .Setup(c => c.Subscribe(It.IsAny<Action<ChannelMessage>>(), It.IsAny<RedisChannel>(), It.IsAny<CommandFlags>()))
        .Returns<Action<ChannelMessage>, RedisChannel, CommandFlags>((x, y, z) =>
        {
          onMessage = x;
          return Task.CompletedTask;
        });

      //Act
      await ClassUnderTest.Subscribe();

      //Assert
      onMessage.Should().NotBeNull();
    }

    [TestMethod]
    public void OnMessageReceived_Insert_CreatedWasPublished()
    {
      //Arrange

      //Act
      ClassUnderTest.PushMessage(ChangeType.Insert);

      //Assert
      VerifyPublish(Enums.ChangeType.Create);
    }

    [TestMethod]
    public void OnMessageReceived_Update_UpdateWasPublished()
    {
      //Arrange

      //Act
      ClassUnderTest.PushMessage(ChangeType.Update);

      //Assert
      VerifyPublish(Enums.ChangeType.Update);
    }

    [TestMethod]
    public void OnMessageReceived_Delete_DeleteWasPublished()
    {
      //Arrange

      //Act
      ClassUnderTest.PushMessage(ChangeType.Delete);

      //Assert
      VerifyPublish(Enums.ChangeType.Delete);
    }

    private void VerifyPublish(Enums.ChangeType changeType)
    {
      MockingKernel.GetMock<IPublisherWithStatus<TestModel>>()
        .Verify(c => c.Publish(It.Is<EntityChange<TestModel>>(ec => ec.ChangeType == changeType)), Times.Once);
    }

    #endregion

    #region TableDependencyStatuses

    [TestMethod]
    public void OnStatusMessageReceived()
    {
      //Arrange
      var status = TableDependencyStatuses.Started;

      //Act
      ClassUnderTest.PushStatusMessage(status);

      //Assert
      VerifyStatusPublish(status);
    }
    
    private void VerifyStatusPublish(TableDependencyStatuses tableDependencyStatuses)
    {
      MockingKernel.GetMock<IPublisherWithStatus<TestModel>>()
        .Verify(c => c.PublishStatus(It.Is<VersionedTableDependencyStatus>(ec => ec.TableDependencyStatus == tableDependencyStatuses)), Times.Once);
    } 

    #endregion

    [TestMethod]
    [ExpectedException(typeof(ObjectDisposedException))]
    public async Task Dispose_CannotSubscribeToDisposed()
    {
      //Arrange
      ClassUnderTest.Dispose();
      
      //Act
      await ClassUnderTest.Subscribe();

      //Assert
    }

    #endregion
  }
}