using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using FluentAssertions;
using Joker.Contracts;
using Joker.Enums;
using Joker.Factories.Schedulers;
using Joker.Notifications;
using Joker.Redis.ConnectionMultiplexers;
using Joker.Redis.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
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

    private ISubject<bool> whenIsConnectedChangesSubject;

    private TableDependencyStatuses initialStatus = TableDependencyStatuses.Started;

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      MockingKernel.Bind<ISchedulersFactory>().ToConstant(SchedulersFactory);

      whenIsConnectedChangesSubject = new Subject<bool>();

      MockingKernel.GetMock<IRedisSubscriber>()
        .Setup(c => c.WhenIsConnectedChanges)
        .Returns(whenIsConnectedChangesSubject);

      MockingKernel.GetMock<IRedisSubscriber>()
        .Setup(c => c.Subscribe(It.IsAny<Action<ChannelMessage>>(), It.IsAny<RedisChannel>(), It.IsAny<CommandFlags>()))
        .Returns(Task.CompletedTask);

      MockingKernel.GetMock<IRedisSubscriber>()
        .Setup(c => c.GetStringAsync($"{typeof(TestModel).Name}-Status"))
        .Returns(Task.FromResult(CreateStatusJson(initialStatus)));

      ClassUnderTest = MockingKernel.Get<TestableDomainEntitiesSubscriber>();
    }

    #endregion

    #region Tests

    private Mock<IRedisSubscriber> RedisSubscriberMock => MockingKernel.GetMock<IRedisSubscriber>();

    #region EntityChange

    [TestMethod]
    public async Task Subscribe_SubscribedToWhenIsConnectedChanges()
    {
      //Arrange
      
      //Act
      await ClassUnderTest.Subscribe();

      //Assert
      RedisSubscriberMock
        .Verify(c => c.WhenIsConnectedChanges, Times.Once);
    }

    [TestMethod]
    public async Task Subscribe_SubscribedToRedisChannel()
    {
      //Arrange
      
      //Act
      await ClassUnderTest.Subscribe();

      //Assert
      RedisSubscriberMock
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
      MockingKernel.GetMock<IEntityChangePublisherWithStatus<TestModel>>()
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
      MockingKernel.GetMock<IEntityChangePublisherWithStatus<TestModel>>()
        .Verify(c => c.Publish(It.Is<VersionedTableDependencyStatus>(ec => ec.TableDependencyStatus == tableDependencyStatuses)), Times.Once);
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

    [TestMethod]
    public void Dispose_UnSubscribedFromRedis()
    {
      //Arrange
      
      //Act
      ClassUnderTest.Dispose();

      //Assert
      MockingKernel.GetMock<IRedisSubscriber>()
        .Verify(c => c.Unsubscribe(It.IsAny<RedisChannel>(), null, CommandFlags.None), Times.Once);
    }

    #endregion

    private string StatusChannelName => $"{typeof(TestModel).Name}-Status";

    [TestMethod]
    public async Task OnRedisConnected_SubscribedToStatusChanges()
    {
      //Arrange
      await ClassUnderTest.Subscribe();
      
      //Act
      whenIsConnectedChangesSubject.OnNext(true);
      RunSchedulers();

      //Assert
      RedisSubscriberMock
        .Verify(c => c.Subscribe(It.IsAny<Action<ChannelMessage>>(), It.Is<RedisChannel>(d => d == StatusChannelName), It.IsAny<CommandFlags>()), Times.Once);
    }

    [TestMethod]
    public async Task OnRedisConnected_LastStatusWasPublished()
    {
      //Arrange
      await ClassUnderTest.Subscribe();
      
      //Act
      whenIsConnectedChangesSubject.OnNext(true);
      RunSchedulers();

      //Assert
      MockingKernel.GetMock<IEntityChangePublisherWithStatus<TestModel>>()
        .Verify(c => c.Publish(It.Is<VersionedTableDependencyStatus>(s => s.TableDependencyStatus == initialStatus)));
    }

    [TestMethod]
    public async Task OnRedisDisconnected_ResetStatusWasPublished()
    {
      //Arrange
      await ClassUnderTest.Subscribe();
      
      //Act
      whenIsConnectedChangesSubject.OnNext(false);
      RunSchedulers();

      //Assert
      MockingKernel.GetMock<IEntityChangePublisherWithStatus<TestModel>>()
        .Verify(c => c.Publish(It.Is<VersionedTableDependencyStatus>(s => s.TableDependencyStatus == TableDependencyStatuses.None && s.Timestamp == DateTimeOffset.MinValue)));
    }

    [TestMethod]
    public async Task Dispose()
    {
      //Arrange
      await ClassUnderTest.Subscribe();
      whenIsConnectedChangesSubject.OnNext(true);
      RunSchedulers();
      
      //Act
      ClassUnderTest.Dispose();

      //Assert
      MockingKernel.GetMock<IRedisSubscriber>()
        .Verify(c => c.Unsubscribe(StatusChannelName, null, CommandFlags.None), Times.Once);
    }

    #region Methods
    
    private string CreateStatusJson(TableDependencyStatuses tableDependencyStatuses)
    {
      var status = new VersionedTableDependencyStatus(tableDependencyStatuses, DateTimeOffset.Now);
      
      var jsonStatus = JsonConvert.SerializeObject(status);

      return jsonStatus;
    }

    #endregion
  }
}