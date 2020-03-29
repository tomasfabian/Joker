using System;
using System.Linq;
using Joker.MVVM.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;
using FluentAssertions;
using Joker.Enums;
using Joker.MVVM.Tests.Helpers;
using System.Reactive.Concurrency;

namespace Joker.MVVM.Tests.ViewModels
{
  [TestClass]
  public class ReactiveListViewModelTests : TestBase<ReactiveListTestViewModel>
  {
    #region TestInitialize

    private readonly TestModelReactiveData reactiveTest = new TestModelReactiveData();

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      ClassUnderTest = CreateReactiveListTestViewModel();

      Init();

      SubscribeToDataChanges();
    }

    private TestModel model2;
    private TestModel model3;
    private TestModel model4;

    private void Init()
    {
      model2 = originalModel.Clone();
      model2.Id = 2;      

      model3 = originalModel.Clone();
      model3.Id = 3;

      model4 = originalModel.Clone();
      model4.Id = 4;
    }

    private ReactiveListTestViewModel CreateReactiveListTestViewModel()
    {
      return new ReactiveListTestViewModel(reactiveTest, schedulersFactory);
    }

    private void SubscribeToDataChanges()
    {
      ClassUnderTest.SubscribeToDataChanges();

      LoadEntitiesFromQuery();
    }

    private void LoadEntitiesFromQuery()
    {
      schedulersFactory.ThreadPool.AdvanceBy(100);
      schedulersFactory.Dispatcher.AdvanceBy(100);
    }

    #endregion

    #region Tests
    
    [TestMethod]
    public void IsLoading()
    {
      //Arrange
      ClassUnderTest.Dispose();
      
      ClassUnderTest = CreateReactiveListTestViewModel();
      
      ClassUnderTest.SubscribeToDataChanges();
      ClassUnderTest.IsLoading.Should().BeTrue();

      //Act
      LoadEntitiesFromQuery();

      //Assert
      ClassUnderTest.IsLoading.Should().BeFalse();
    }

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
      var viewModel = ClassUnderTest.Find(originalModel);

      viewModel.Should().NotBeNull();
      viewModel.Name.Should().NotBe(originalModel.Name);
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
      var model = originalModel.Clone();

      model.Name = null;
      model.Timestamp = originalModel.Timestamp.AddHours(-1);

      //Act
      PublishEntityUpdatedEvent(model);

      //Assert
      ClassUnderTest.Items.Count.Should().Be(2);
      var viewModel = ClassUnderTest.Find(originalModel);

      viewModel.Should().NotBeNull();
    }        

    [TestMethod]
    public void UpdateForNotFoundModel_CanNotAddMissingEntityOnUpdate_ViewModelWasCreated()
    {
      //Arrange
      ClassUnderTest.CanAddMissingEntityOnUpdateOverride = false;
      var model = originalModel.Clone();

      model.Name = null;
      model.Timestamp = originalModel.Timestamp.AddHours(-1);

      //Act
      PublishEntityUpdatedEvent(model);

      //Assert
      ClassUnderTest.Items.Count.Should().Be(1);
      var viewModel = ClassUnderTest.Find(originalModel);

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
      var viewModel = ClassUnderTest.Find(originalModel);

      viewModel.Should().NotBeNull();
      viewModel.Name.Should().NotBe(originalModel.Name);
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
      SchedulePublishChangeEvent(ChangeType.Create, model2, 100);
      SchedulePublishChangeEvent(ChangeType.Create, model3, 200);

      SchedulePublishChangeEvent(ChangeType.Create, model4, defaultBufferTimeSpan);
      
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
      SchedulePublishChangeEvent(ChangeType.Create, model2, 100);
      SchedulePublishChangeEvent(ChangeType.Create, model3, 200);
      SchedulePublishChangeEvent(ChangeType.Create, model4, 300);
      
      ClassUnderTest.Items.Count.Should().Be(1);

      //Act
      AdvanceChangesBuffer(300);

      //Assert
      ClassUnderTest.Items.Count.Should().Be(4);
    }

    [TestMethod]
    public void LoadingData_OnCreatedWasBuffered()
    {
      //Arrange
      ClassUnderTest.Dispose();
      ClassUnderTest = CreateReactiveListTestViewModel();
      ClassUnderTest.SubscribeToDataChanges();

      SchedulePublishChangeEvent(ChangeType.Create, model2, 10);
      AdvanceChangesBuffer(20);
      
      ClassUnderTest.Items.Count.Should().Be(0);

      //Act
      LoadEntitiesFromQuery();
      AdvanceChangesBuffer();

      //Assert
      ClassUnderTest.Items.Count.Should().Be(2);
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

    #endregion

    #region Methods

    readonly TimeSpan defaultBufferTimeSpan = TimeSpan.FromMilliseconds(250);

    private void AdvanceChangesBuffer(long bufferTicks)
    {
      schedulersFactory.TaskPool.AdvanceBy(bufferTicks);
      schedulersFactory.Dispatcher.AdvanceBy(bufferTicks);
    }    
    
    private void AdvanceChangesBuffer()
    {
      AdvanceChangesBuffer(defaultBufferTimeSpan.Ticks);
    }

    private void SchedulePublishChangeEvent(ChangeType changeType, TestModel model, long ticks)
    {
      SchedulePublishChangeEvent(changeType, model, TimeSpan.FromTicks(ticks));
    }

    private void SchedulePublishChangeEvent(ChangeType changeType, TestModel model, TimeSpan timeSpan)
    {
      schedulersFactory.TaskPool.Schedule(timeSpan, () => { PublishChangeEvent(changeType, model); });
    }

    private void PublishChangeEvent(ChangeType changeType, TestModel model)
    {
      reactiveTest.Publish(new EntityChange<TestModel>
      {
        ChangeType = changeType,
        Entity = model
      });
    }

    private void PublishChangeEventAndAdvanceTime(ChangeType changeType, TestModel model)
    {
      PublishChangeEvent(changeType, model);

      AdvanceChangesBuffer();
    }

    private readonly TestModel originalModel = new TestModel
    {
      Id = 2,
      Timestamp = new DateTime(2020, 3, 27, 16, 0, 0),
      Name = "Original"
    };

    private void PublishEntityAddedEvent()
    {
      PublishChangeEventAndAdvanceTime(ChangeType.Create, originalModel);
    }

    private void PublishEntityUpdatedEvent(TestModel model = null)
    {
      if(model == null)
      {
        model = originalModel.Clone();

        model.Name = "Renamed";
        model.Timestamp = originalModel.Timestamp.AddDays(1);
      }
      
      PublishChangeEventAndAdvanceTime(ChangeType.Update, model);
    }

    private void PublishEntityDeletedEvent()
    {
      var clone = originalModel.Clone();

      clone.Timestamp = originalModel.Timestamp.AddMonths(1);

      PublishChangeEventAndAdvanceTime(ChangeType.Delete, clone);
    }

    #endregion

    #region CleanUp

    [TestCleanup]
    public void CleanUp()
    {
    }

    #endregion
  }
}