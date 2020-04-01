using System;
using System.Linq;
using FluentAssertions;
using Joker.MVVM.Tests.Helpers;
using Joker.MVVM.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;

namespace Joker.MVVM.Tests.ViewModels
{
  [TestClass]
  public class ViewModelsListTests : TestBase<TestModelsViewModel>
  {
    #region TestInitialize

    protected readonly TestModelProvider TestModelProvider = new TestModelProvider();

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      ClassUnderTest = new TestModelsViewModel();

      ClassUnderTest.Add(TestModelProvider.Model1);
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

    #region Sorting
    
    [TestMethod]
    public void Sort_ItemsAreOrderedByNameDescThenByIdAsc()
    {
      //Arrange
      var model2 = TestModelProvider.Model2;
      model2.Name = "z";
      var model3 = TestModelProvider.Model3;
      model3.Name = "z";

      //Act
      ClassUnderTest.Add(model2);
      ClassUnderTest.Add(model3);

      //Assert
      var expectedOrder = new[] {TestModelProvider.Model2, TestModelProvider.Model3, TestModelProvider.Model1}
        .Select(c => c.Id);
      ClassUnderTest.Items.Select(c => c.Id).SequenceEqual(expectedOrder).Should().BeTrue();
    }
    
    [TestMethod]
    public void Sort_ReverseOrder()
    {
      //Arrange
      ClassUnderTest.Add(TestModelProvider.Model2);
      var model3 = TestModelProvider.Model3;
      model3.Name = "a";
      ClassUnderTest.Add(model3);
      var originalOrder = ClassUnderTest.Items.ToList();

      //Act
      ClassUnderTest.SetAscendingNameSortDescription();

      //Assert
      originalOrder.Reverse();
      ClassUnderTest.Items.ToList().SequenceEqual(originalOrder).Should().BeTrue();
    }

    #endregion
  }
}