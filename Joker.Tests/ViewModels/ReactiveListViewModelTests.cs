using System;
using System.Linq;
using Joker.MVVM.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Joker.Enums;
using Joker.MVVM.Tests.Helpers;

namespace Joker.MVVM.Tests.ViewModels
{
  [TestClass]
  public class ReactiveListViewModelTests : ReactiveListViewModelTestsBase
  {
    #region TestInitialize

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      ClassUnderTest = CreateReactiveListTestViewModel();
      
      SubscribeToDataChanges();
    }

    #endregion

    #region Tests

    #region EntityChanges

    [TestMethod]
    public void OnEntityCreationWasPublished()
    {
      //Arrange

      //Act
      PublishEntityAddedEvent();

      //Assert
      ClassUnderTest.Items.Count.Should().Be(2);
    }

    [TestMethod]
    public void OnEntityUpdateWasPublished()
    {
      //Arrange
      PublishEntityAddedEvent();

      //Act
      PublishEntityUpdatedEvent();

      //Assert
      ClassUnderTest.Items.Count.Should().Be(2);
      var viewModel = ClassUnderTest.Find(OriginalModel);

      viewModel.Should().NotBeNull();
      viewModel.Name.Should().NotBe(OriginalModel.Name);
    }
    
    [TestMethod]
    public void OnOlderEntityUpdateWasPublished_ViewModelWasNotUpdated()
    {
      //Arrange
      var model = ClassUnderTest.Items.First().Model.Clone();

      model.Name = null;
      model.Timestamp = model.Timestamp.AddHours(-1);

      //Act
      PublishEntityUpdatedEvent(model);

      //Assert
      ClassUnderTest.Items.Count.Should().Be(1);
      var viewModel = ClassUnderTest.Find(model);

      viewModel.Should().NotBeNull();
      viewModel.Name.Should().NotBe(model.Name);
    }
        
    [TestMethod]
    public void UpdateForNotFoundModel_CanAddMissingEntityOnUpdate_ViewModelWasCreated()
    {
      //Arrange
      var model = OriginalModel.Clone();

      model.Name = null;
      model.Timestamp = OriginalModel.Timestamp.AddHours(-1);

      //Act
      PublishEntityUpdatedEvent(model);

      //Assert
      ClassUnderTest.Items.Count.Should().Be(2);
      var viewModel = ClassUnderTest.Find(OriginalModel);

      viewModel.Should().NotBeNull();
    }        

    [TestMethod]
    public void UpdateForNotFoundModel_CanNotAddMissingEntityOnUpdate_ViewModelWasCreated()
    {
      //Arrange
      ClassUnderTest.CanAddMissingEntityOnUpdateOverride = false;
      var model = OriginalModel.Clone();

      model.Name = null;
      model.Timestamp = OriginalModel.Timestamp.AddHours(-1);

      //Act
      PublishEntityUpdatedEvent(model);

      //Assert
      ClassUnderTest.Items.Count.Should().Be(1);
      var viewModel = ClassUnderTest.Find(OriginalModel);

      viewModel.Should().BeNull();
    }

    [TestMethod]
    public void HasUpdateAction_ViewModelWasUpdated()
    {
      //Arrange
      ClassUnderTest.CreateUpdateViewModelAction = true;

      PublishEntityAddedEvent();

      //Act
      PublishEntityUpdatedEvent();

      //Assert
      ClassUnderTest.Items.Count.Should().Be(2);
      var viewModel = ClassUnderTest.Find(OriginalModel);

      viewModel.Should().NotBeNull();
      viewModel.Name.Should().NotBe(OriginalModel.Name);
    }
    
    [TestMethod]
    public void OnEntityDeleteWasPublished()
    {
      //Arrange
      PublishEntityAddedEvent();

      //Act
      PublishEntityDeletedEvent();

      //Assert
      ClassUnderTest.Items.Count.Should().Be(1);
    }

    #endregion

    #region DataChange buffering

    [TestMethod]
    public void ThirdEntityCreationWasPublishedAfterBufferTimeSpan()
    {
      //Arrange
      SchedulePublishChangeEvent(ChangeType.Create, Model2, 100);
      SchedulePublishChangeEvent(ChangeType.Create, Model3, 200);

      SchedulePublishChangeEvent(ChangeType.Create, Model4, DefaultBufferTimeSpan);
      
      ClassUnderTest.Items.Count.Should().Be(1);

      //Act
      AdvanceChangesBuffer();

      //Assert
      ClassUnderTest.Items.Count.Should().Be(3);

      //Act
      AdvanceChangesBuffer();

      //Assert
      ClassUnderTest.Items.Count.Should().Be(4);
    }

    [TestMethod]
    public void DataChangesBufferCountWasReachedBeforeTimeSpan()
    {
      //Arrange
      SchedulePublishChangeEvent(ChangeType.Create, Model2, 100);
      SchedulePublishChangeEvent(ChangeType.Create, Model3, 200);
      SchedulePublishChangeEvent(ChangeType.Create, Model4, 300);
      
      ClassUnderTest.Items.Count.Should().Be(1);

      //Act
      AdvanceChangesBuffer(300);

      //Assert
      ClassUnderTest.Items.Count.Should().Be(4);
    }

    #endregion

    #region SelectionChanged

    [TestMethod]
    public void SelectionChanged_NotificationIsPublished()
    {
      //Arrange
      SelectionChangedEventArgs<TestViewModel> selectionChangedEventArgs = null;
      var subscription = ClassUnderTest.SelectionChanged.Subscribe(ea => selectionChangedEventArgs = ea);

      //Act
      ClassUnderTest.SelectedItem = ClassUnderTest.Items.FirstOrDefault();

      //Assert
      selectionChangedEventArgs.Should().NotBeNull();
      selectionChangedEventArgs.OldValue.Should().BeNull();
      selectionChangedEventArgs.NewValue.Should().Be(ClassUnderTest.SelectedItem);

      subscription.Dispose();
    }

    [TestMethod]
    public void LastSelectionChangeWasObserved()
    {
      //Arrange
      ClassUnderTest.SelectedItem = ClassUnderTest.Items.FirstOrDefault();

      SelectionChangedEventArgs<TestViewModel> selectionChangedEventArgs = null;
      var subscription = ClassUnderTest.SelectionChanged.Subscribe(ea => selectionChangedEventArgs = ea);

      //Act
      ClassUnderTest.SelectedItem = ClassUnderTest.Items.FirstOrDefault();

      //Assert
      selectionChangedEventArgs.Should().NotBeNull();

      subscription.Dispose();
    }

    [TestMethod]
    public void SelectionChangedSecondTime_OldValueWasPublished()
    {
      //Arrange
      var originalSelectedItem = ClassUnderTest.SelectedItem = ClassUnderTest.Items.FirstOrDefault();

      SelectionChangedEventArgs<TestViewModel> selectionChangedEventArgs = null;
      var subscription = ClassUnderTest.SelectionChanged.Subscribe(ea => selectionChangedEventArgs = ea);

      //Act
      ClassUnderTest.SelectedItem = null;

      //Assert
      selectionChangedEventArgs.Should().NotBeNull();
      selectionChangedEventArgs.OldValue.Should().Be(originalSelectedItem);
      selectionChangedEventArgs.NewValue.Should().BeNull();

      subscription.Dispose();
    }

    #endregion

    #region SubscribeSecondTime

    [TestMethod]
    public void SubscribeSecondTime_EverythingShouldBeReset()
    {
      //Arrange
      
      //Act
      ClassUnderTest.SubscribeToDataChanges();

      //Assert
      ClassUnderTest.IsLoading.Should().BeTrue();
      ClassUnderTest.Items.Should().BeEmpty();
    }

    [TestMethod]
    public void SubscribeSecondTime_ItemsWereReloadedAndObserved()
    {
      //Arrange
      ClassUnderTest.SubscribeToDataChanges();
      
      //Act
      LoadEntitiesFromQuery();

      //Assert
      ClassUnderTest.IsLoading.Should().BeFalse();
      ClassUnderTest.Items.Count.Should().Be(1);

      //Act
      SchedulePublishChangeEvent(ChangeType.Create, Model2, 100);
      AdvanceChangesBuffer();

      //Assert
      ClassUnderTest.Items.Count.Should().Be(2);
    }

    #endregion

    #region Dispose
    
    [TestMethod]
    public void Dispose_ItemsWereCleared()
    {
      //Arrange
      
      //Act
      ClassUnderTest.Dispose();

      //Assert
      ClassUnderTest.Items.Should().BeEmpty();
    }
    
    [TestMethod]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void Dispose_CannotSubscribeToDisposed()
    {
      //Arrange
      ClassUnderTest.Dispose();
      
      //Act
      ClassUnderTest.SubscribeToDataChanges();

      //Assert
    }

    #endregion

    #endregion

    #region CleanUp

    [TestCleanup]
    public void CleanUp()
    {
      using (ClassUnderTest)
      {
      }
    }

    #endregion
  }
}