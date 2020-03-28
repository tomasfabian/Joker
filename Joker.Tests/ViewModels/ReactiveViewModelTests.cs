using System;
using System.Linq;
using System.Reactive.Linq;
using Joker.MVVM.Enums;
using Joker.MVVM.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;
using FluentAssertions;

namespace Joker.MVVM.Tests.ViewModels
{
  [TestClass]
  public class ReactiveViewModelTests : TestBase<ReactiveViewModel<TestModel, TestViewModel>>
  {
    #region TestInitialize

    private readonly ReactiveTest reactiveTest = new ReactiveTest();

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      ClassUnderTest = new ReactiveTestViewModel(reactiveTest, schedulersFactory);

      ClassUnderTest.SubscribeToDataChanges();
    }

    #endregion

    #region Tests

    [TestMethod]
    public void OnEntityCreationWasPublished()
    {
      //Arrange

      //Act
      PublishEntityAddedEvent();
      RunSchedulers();

      //Assert
      ClassUnderTest.Items.Count.Should().Be(1);
    }

    [TestMethod]
    public void OnEntityUpdateWasPublished()
    {
      //Arrange
      PublishEntityAddedEvent();
      RunSchedulers();

      //Act
      PublishEntityUpdatedEvent();
      RunSchedulers();

      //Assert
      ClassUnderTest.Items.Count.Should().Be(1);
      var viewModel = ClassUnderTest.Find(originalModel);

      viewModel.Should().NotBeNull();
      viewModel.Name.Should().NotBe(originalModel.Name);
    }
    
    [TestMethod]
    public void OnEntityDeleteWasPublished()
    {
      //Arrange
      PublishEntityAddedEvent();
      RunSchedulers();

      //Act
      PublishEntityDeletedEvent();
      RunSchedulers();

      //Assert
      ClassUnderTest.Items.Count.Should().Be(0);
    }

    [TestMethod]
    public void SelectionChanged_NotificationIsPublished()
    {
      //Arrange
      PublishEntityAddedEvent();
      RunSchedulers();

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
      PublishEntityAddedEvent();
      RunSchedulers();
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
      PublishEntityAddedEvent();
      RunSchedulers();
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

    #region Methods

    private void PublishChangeEvent(ChangeType changeType, TestModel model)
    {
      reactiveTest.Publish(new EntityChange<TestModel>
      {
        ChangeType = changeType,
        Entity = model
      });
    }

    private readonly TestModel originalModel = new TestModel
    {
      Id = 1,
      Timestamp = new DateTime(2020, 3, 27, 16, 0, 0),
      Name = "Original"
    };

    private void PublishEntityAddedEvent()
    {
      PublishChangeEvent(ChangeType.Create, originalModel);
    }

    private void PublishEntityUpdatedEvent()
    {
      var clone = originalModel.Clone();

      clone.Name = "Renamed";
      clone.Timestamp = originalModel.Timestamp.AddDays(1);

      PublishChangeEvent(ChangeType.Update, clone);
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