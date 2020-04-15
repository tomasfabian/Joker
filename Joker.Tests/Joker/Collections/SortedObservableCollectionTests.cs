using System;
using System.Collections.Generic;
using FluentAssertions;
using Joker.Collections;
using Joker.Comparators;
using Joker.MVVM.Tests.Helpers;
using Joker.MVVM.ViewModels.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;

namespace Joker.MVVM.Tests.Joker.Collections
{
  [TestClass]
  public class SortedObservableCollectionTests : TestBase<SortedObservableCollection<TestViewModel>>
  {
    private readonly IComparer<DomainEntityViewModel<TestModel>> comparer = new GenericComparer<DomainEntityViewModel<TestModel>>((x, y) =>
      Comparer<int>.Default.Compare(x.Model.Id, y.Model.Id));

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      ClassUnderTest = new SortedObservableCollection<TestViewModel>(comparer);
    }

    private TestViewModel CreateItem(int id)
    {
      return new TestViewModel(new TestModel {Id = id});
    }

    private void AddIdRange(Range range)
    {
      (int offset, int length) = range.GetOffsetAndLength(range.End.Value);

      for (var id = range.Start.Value; id <= offset + length; id++)
      {
        ClassUnderTest.Add(CreateItem(id));
      }
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AddNull()
    {
      //Act
      ClassUnderTest.Add(null);
    }

    [TestMethod]
    public void Ctor_FillWithValuesFromAnotherCollection()
    {
      //Arrange    
      AddIdRange(new Range(2, 5));
      
      //Act
      var collection = new SortedObservableCollection<DomainEntityViewModel<TestModel>>(comparer, ClassUnderTest);  

      //Assert
      collection.Count.Should().Be(ClassUnderTest.Count);
    }

    [TestMethod]
    public void AddId1_ShouldBeAddedFirst()
    {
      //Arrange
      var range = new Range(2, 5);

      AddIdRange(range);
      int expectedItemsCount = ClassUnderTest.Count + 1;

      //Act
      ClassUnderTest.Add(CreateItem(1));

      //Assert
      ClassUnderTest[0].Id.Should().Be(1);
      ClassUnderTest.Count.Should().Be(expectedItemsCount);
    }

    [TestMethod]
    public void Remove_OneLessItem()
    {
      //Arrange
      AddIdRange(new Range(2, 5));
      var itemToRemove = ClassUnderTest[3];

      //Act
      ClassUnderTest.Remove(ClassUnderTest[1]);
      ClassUnderTest.Remove(itemToRemove);
      ClassUnderTest.Remove(ClassUnderTest[1]);
      ClassUnderTest.Remove(ClassUnderTest[0]);

      //Assert
      ClassUnderTest.Should().NotContain(itemToRemove);
    }

    [TestMethod]
    public void RemoveNotExisting_NothingHappens()
    {
      //Arrange
      AddIdRange(new Range(2, 5));
      int expectedItemsCount = ClassUnderTest.Count;

      //Act
      ClassUnderTest.Remove(CreateItem(-1));

      //Assert
      ClassUnderTest.Count.Should().Be(expectedItemsCount);
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void SetItem()
    {
      //Arrange
      AddIdRange(new Range(2, 5));

      //Act
      ClassUnderTest[2] = CreateItem(1);

      //Assert
    }
    
    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void MoveItem()
    {
      //Arrange
      AddIdRange(new Range(2, 5));

      //Act
      ClassUnderTest.Move(1, 0);

      //Assert
    }

    [TestMethod]
    public void ClearAndAdd_ShouldBeAddedFirst()
    {
      //Arrange
      AddIdRange(new Range(2, 5));
      ClassUnderTest.Clear();

      //Act
      ClassUnderTest.Add(CreateItem(1));

      //Assert
      ClassUnderTest[0].Id.Should().Be(1);
      ClassUnderTest.Count.Should().Be(1);
    }

    [TestMethod]
    public void Clear_ShouldBeEmpty()
    {
      //Arrange
      AddIdRange(new Range(2, 5));

      //Act
      ClassUnderTest.Clear();

      //Assert
      ClassUnderTest.Should().BeEmpty();
    }

    [TestMethod]
    public void UpdateKeyField_CollectionWasReordered()
    {
      //Arrange    
      AddIdRange(new Range(2, 5));
      var item = ClassUnderTest[1];

      //Act
      item.TestId = -item.TestId;

      //Assert
      ClassUnderTest.IndexOf(item).Should().Be(0);

      //Act
      item.TestId = int.MaxValue;

      //Assert
      ClassUnderTest.IndexOf(item).Should().Be(3);
    }
  }
}