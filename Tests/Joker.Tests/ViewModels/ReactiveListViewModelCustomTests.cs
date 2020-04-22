using System.Linq;
using FluentAssertions;
using Joker.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Joker.MVVM.Tests.ViewModels
{
  [TestClass]
  public class ReactiveListViewModelCustomTests : ReactiveListViewModelTestsBase
  {
    #region Tests

    [TestMethod]
    public void IsLoading()
    {
      //Arrange
      ClassUnderTest = CreateReactiveListTestViewModel();
      
      ClassUnderTest.SubscribeToDataChanges();
      ClassUnderTest.IsLoading.Should().BeTrue();

      //Act
      LoadEntitiesFromQuery();

      //Assert
      ClassUnderTest.IsLoading.Should().BeFalse();
    }
    
    [TestMethod]
    public void LoadingData_OnCreatedWasBuffered()
    {
      //Arrange
      ClassUnderTest = CreateReactiveListTestViewModel();
      ClassUnderTest.SubscribeToDataChanges();

      SchedulePublishChangeEvent(ChangeType.Create, TestModelProvider.Model2, 10);
      AdvanceChangesBuffer(20);
      
      ClassUnderTest.Items.Count.Should().Be(0);

      //Act
      LoadEntitiesFromQuery();
      AdvanceChangesBuffer();

      //Assert
      ClassUnderTest.Items.Count.Should().Be(2);
    }

    #region Filtering
    
    [TestMethod]
    public void SetFilter_QueryLoaded_ItemWasNotAdded()
    {
      //Arrange
      ClassUnderTest = CreateReactiveListTestViewModel();
      ClassUnderTest.SetModelsFilter(m => m.Id != TestModelProvider.Model1.Id);

      //Act
      SubscribeToDataChanges();

      //Assert
      ClassUnderTest.Items.Should().BeEmpty();
    }

    [TestMethod]
    public void SetFilter_PushedDataChanges_ItemWasNotAdded()
    {
      //Arrange
      ClassUnderTest = CreateReactiveListTestViewModel();
      ClassUnderTest.SetModelsFilter(m => m.Id != TestModelProvider.Model2.Id);
      
      SubscribeToDataChanges();

      SchedulePublishChangeEvent(ChangeType.Create, TestModelProvider.Model2, 100);
      SchedulePublishChangeEvent(ChangeType.Create, TestModelProvider.Model3, 200);

      //Act
      AdvanceChangesBuffer();

      //Assert
      ClassUnderTest.Items.Count.Should().Be(2);
      ClassUnderTest.Items.FirstOrDefault(c => c.Id == TestModelProvider.Model2.Id).Should().BeNull();
    }

    #endregion

    #endregion
  }
}