using System;
using System.Linq;
using Joker.MVVM.Enums;
using Joker.MVVM.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;
using FluentAssertions;
using Joker.MVVM.Tests.Helpers;

namespace Joker.MVVM.Tests.ViewModels
{
  [TestClass]
  public class ReactiveListViewModelTests : TestBase<ReactiveListTestViewModel>
  {
    #region TestInitialize

    private readonly ReactiveTest reactiveTest = new ReactiveTest();

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      ClassUnderTest = new ReactiveListTestViewModel(reactiveTest, schedulersFactory);

      SubscribeToDataChanges();
    }

    private void SubscribeToDataChanges()
    {
      ClassUnderTest.SubscribeToDataChanges();

      //load entities
      schedulersFactory.ThreadPool.AdvanceBy(100);
      schedulersFactory.Dispatcher.AdvanceBy(100);

      AdvanceChangesBuffer();
    }

    #endregion

    #region Tests
    
    [TestMethod]
    public void IsLoading()
    {
      //Arrange
      ClassUnderTest.Dispose();
      
      ClassUnderTest = new ReactiveListTestViewModel(reactiveTest, schedulersFactory);
      
      ClassUnderTest.SubscribeToDataChanges();
      ClassUnderTest.IsLoading.Should().BeTrue();

      //Act
      schedulersFactory.ThreadPool.AdvanceBy(100);
      schedulersFactory.Dispatcher.AdvanceBy(100);

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
    public void UpdateForNotExistingModel_ViewModelWasCreated()
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

    readonly long bufferTicks = TimeSpan.FromMilliseconds(250).Ticks;

    private void AdvanceChangesBuffer()
    {
      schedulersFactory.TaskPool.AdvanceBy(bufferTicks);
      schedulersFactory.Dispatcher.AdvanceBy(bufferTicks);
    }

    private void PublishChangeEvent(ChangeType changeType, TestModel model)
    {
      reactiveTest.Publish(new EntityChange<TestModel>
      {
        ChangeType = changeType,
        Entity = model
      });

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
      PublishChangeEvent(ChangeType.Create, originalModel);
    }

    private void PublishEntityUpdatedEvent(TestModel model = null)
    {
      if(model == null)
      {
        model = originalModel.Clone();

        model.Name = "Renamed";
        model.Timestamp = originalModel.Timestamp.AddDays(1);
      }
      
      PublishChangeEvent(ChangeType.Update, model);
    }

    private void PublishEntityDeletedEvent()
    {
      var clone = originalModel.Clone();

      clone.Timestamp = originalModel.Timestamp.AddMonths(1);

      PublishChangeEvent(ChangeType.Delete, clone);
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