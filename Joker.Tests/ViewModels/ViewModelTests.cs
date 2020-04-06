using System;
using FluentAssertions;
using Joker.MVVM.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;

namespace Joker.MVVM.Tests.ViewModels
{
  [TestClass]
  public class ViewModelTests : TestBase<TestViewModel>
  {
    #region TestInitialize

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      ClassUnderTest = new TestViewModel(new TestModel { Name = nameof(TestModel) });
    }

    #endregion

    [TestMethod]
    public void PropertyChange()
    {
      //Arrange
      string newTestPropertyValue = "changed";

      //Act
      ClassUnderTest.TestProperty = newTestPropertyValue;

      //Assert
      ClassUnderTest.TestProperty.Should().BeEquivalentTo(newTestPropertyValue);
    }

    [TestMethod]
    public void PropertyChange_OnChangedActionWasCalled()
    {
      //Arrange
      string newTestPropertyValue = "changed";
      ClassUnderTest.IsDirty.Should().BeFalse();

      //Act
      ClassUnderTest.TestProperty = newTestPropertyValue;

      //Assert
      ClassUnderTest.IsDirty.Should().BeTrue();
    }

    [TestMethod]
    public void PropertyChange_PropertyChangedNotificationWasRaised()
    {
      //Arrange
      string newTestPropertyValue = "changed";

      string raisedPropertyName = null;
      void OnClassUnderTestPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
        raisedPropertyName = e.PropertyName;
      }

      ClassUnderTest.PropertyChanged += OnClassUnderTestPropertyChanged;

      //Act
      ClassUnderTest.TestProperty = newTestPropertyValue;

      //Assert
      raisedPropertyName.Should().BeEquivalentTo(nameof(TestViewModel.TestProperty));
      ClassUnderTest.PropertyChanged -= OnClassUnderTestPropertyChanged;
    }

    [TestMethod]
    public void PropertyIsSetToSameValue_PropertyChangedNotificationWasNotRaised()
    {
      //Arrange
      string raisedPropertyName = null;
      void OnClassUnderTestPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
        raisedPropertyName = e.PropertyName;
      }

      ClassUnderTest.PropertyChanged += OnClassUnderTestPropertyChanged;

      //Act
      ClassUnderTest.TestProperty = ClassUnderTest.TestProperty;

      //Assert
      raisedPropertyName.Should().BeNull();
      ClassUnderTest.IsDirty.Should().BeFalse();
      ClassUnderTest.PropertyChanged -= OnClassUnderTestPropertyChanged;
    }
  }
}