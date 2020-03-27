using System;
using Joker.MVVM.Enums;
using Joker.MVVM.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;
using FluentAssertions;

namespace Joker.Tests.ViewModels
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
      reactiveTest.Publish(new EntityChange<TestModel>
                           {
                             ChangeType = ChangeType.Create,
                             Entity = new TestModel {  Id = 1, Timestamp = new DateTime(2020, 3, 27, 16, 0, 0)}
                           });
      RunSchedulers();

      //Assert
      Assert.AreEqual(1, ClassUnderTest.Items.Count);
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