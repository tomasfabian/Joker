using System;
using System.Reactive.Concurrency;
using FluentAssertions;
using Joker.Contracts;
using Joker.MVVM.Tests.Helpers;
using Joker.MVVM.ViewModels;
using Joker.Notifications;
using Joker.Reactive;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using UnitTests;

namespace Joker.MVVM.Tests.ViewModels
{
  [TestClass]
  public class EntityChangesViewModelTests : TestBase<EntityChangesViewModel<TestViewModel>>
  {
    #region TestInitialize

    private Mock<IReactiveListViewModel<TestViewModel>> reactiveListViewModelMock;
    private ReactiveDataWithStatus<TestModel> statusPublisher;

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      reactiveListViewModelMock = new Mock<IReactiveListViewModel<TestViewModel>>();
      statusPublisher = new ReactiveDataWithStatus<TestModel>();

      MockingKernel.GetMock<IReactiveListViewModelFactory<TestViewModel>>()
        .Setup(c => c.Create())
        .Returns(reactiveListViewModelMock.Object);

      MockingKernel.Bind<ITableDependencyStatusProvider>()
        .ToConstant(statusPublisher);

      MockingKernel.Rebind<IScheduler>()
        .ToConstant(SchedulersFactory.Dispatcher);

      ClassUnderTest = MockingKernel.Get<EntityChangesViewModel<TestViewModel>>();
    }

    #endregion

    #region Tests

    #region IsOffline

    [TestMethod]
    public void Ctor()
    {        
      //Arrange

      //Act

      //Assert
      ClassUnderTest.IsOffline.Should().BeTrue();
    }

    #endregion

    #region Status changed

    [TestMethod]
    public void StatusStarting_IsOnline()
    {        
      //Arrange
      PublishStatusChanged(VersionedTableDependencyStatus.TableDependencyStatuses.None);
      ClassUnderTest.IsOffline.Should().BeTrue();

      //Act
      PublishStatusChanged(VersionedTableDependencyStatus.TableDependencyStatuses.Starting);

      //Assert
      reactiveListViewModelMock.Verify(c => c.SubscribeToDataChanges(), Times.Never);
      ClassUnderTest.IsOffline.Should().BeTrue();
    }

    [TestMethod]
    public void StatusWaitingForNotification_IsOnline()
    {        
      //Arrange

      //Act
      PublishStatusChanged(VersionedTableDependencyStatus.TableDependencyStatuses.WaitingForNotification);
      RunSchedulers();

      //Assert
      reactiveListViewModelMock.Verify(c => c.SubscribeToDataChanges(), Times.Once);
      ClassUnderTest.IsOffline.Should().BeFalse();
    }

    [TestMethod]
    public void StatusStarted_IsOnline()
    {        
      //Arrange

      //Act
      PublishStatusChanged(VersionedTableDependencyStatus.TableDependencyStatuses.Started);
      RunSchedulers();

      //Assert
      reactiveListViewModelMock.Verify(c => c.SubscribeToDataChanges(), Times.Once);
      ClassUnderTest.IsOffline.Should().BeFalse();
    }

    [TestMethod]
    public void StatusWaitingForNotificationThenStarted_IsOnline()
    {        
      //Arrange
      PublishStatusChanged(VersionedTableDependencyStatus.TableDependencyStatuses.WaitingForNotification);

      //Act
      PublishStatusChanged(VersionedTableDependencyStatus.TableDependencyStatuses.Started);
      RunSchedulers();

      //Assert
      reactiveListViewModelMock.Verify(c => c.SubscribeToDataChanges(), Times.Once);
      ClassUnderTest.IsOffline.Should().BeFalse();
    }
    
    [TestMethod]
    public void StatusStopDueToCancellation_IsOnline()
    {        
      //Arrange

      //Act
      PublishStatusChanged(VersionedTableDependencyStatus.TableDependencyStatuses.StopDueToCancellation);

      //Assert
      reactiveListViewModelMock.Verify(c => c.SubscribeToDataChanges(), Times.Never);
      ClassUnderTest.IsOffline.Should().BeTrue();
    }

    [TestMethod]
    public void StatusStopDueToError_IsOnline()
    {        
      //Arrange

      //Act
      PublishStatusChanged(VersionedTableDependencyStatus.TableDependencyStatuses.StopDueToError);

      //Assert
      reactiveListViewModelMock.Verify(c => c.SubscribeToDataChanges(), Times.Never);
      ClassUnderTest.IsOffline.Should().BeTrue();
    }  
    
    [TestMethod]
    public void OldValueIsIgnored()
    {        
      //Arrange
      PublishStatusChanged(VersionedTableDependencyStatus.TableDependencyStatuses.WaitingForNotification);

      //Act
      PublishStatusChanged(VersionedTableDependencyStatus.TableDependencyStatuses.None, DateTimeOffset.Now.AddHours(-2));
      RunSchedulers();

      //Assert
      reactiveListViewModelMock.Verify(c => c.SubscribeToDataChanges(), Times.Once);
      ClassUnderTest.IsOffline.Should().BeFalse();
    }
    
    [TestMethod]
    public void DateTimeOffset_MinValue_ResetsStateToNone()
    {        
      //Arrange
      PublishStatusChanged(VersionedTableDependencyStatus.TableDependencyStatuses.WaitingForNotification);

      //Act
      PublishStatusChanged(VersionedTableDependencyStatus.TableDependencyStatuses.None, DateTimeOffset.MinValue);
      RunSchedulers();

      //Assert
      reactiveListViewModelMock.Verify(c => c.SubscribeToDataChanges(), Times.Once);
      ClassUnderTest.IsOffline.Should().BeTrue();
    }

    #endregion  

    #region Dispose

    [TestMethod]
    public void Dispose()
    {        
      //Arrange

      //Act
      ClassUnderTest.Dispose();

      //Assert
      reactiveListViewModelMock.Verify(c => c.Dispose(), Times.Never);
    }

    [TestMethod]
    public void SubscribedToChanged_Dispose()
    {        
      //Arrange
      PublishStatusChanged(VersionedTableDependencyStatus.TableDependencyStatuses.Started);
      RunSchedulers();

      //Act
      ClassUnderTest.Dispose();

      //Assert
      reactiveListViewModelMock.Verify(c => c.Dispose(), Times.Once);
    }

    #endregion

    #endregion

    #region Methods

    private void PublishStatusChanged(VersionedTableDependencyStatus.TableDependencyStatuses tableDependencyStatuses, DateTimeOffset? when = null)
    {
      statusPublisher.Publish(
        new VersionedTableDependencyStatus(tableDependencyStatuses, when ?? DateTimeOffset.Now));
    }

    #endregion
  }
}