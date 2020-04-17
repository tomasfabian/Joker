using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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

    #region Clear

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
    public void Clear_OldItemsCollectionChangeNotification()
    {
      //Arrange
      ClassUnderTest.SupportsRangeNotifications = true;
      AddIdRange(new Range(2, 5));
      var expectedItemsCount = ClassUnderTest.Count;

      IList removedItems = new List<TestViewModel>();

      int bulkRemovedItemsCount = 0;

      void Handler(object sender, NotifyCollectionChangedEventArgs args)
      {
        if (args.Action == NotifyCollectionChangedAction.Remove)
        {
          foreach (var oldItem in args.OldItems)
          {
            removedItems.Add(oldItem);
          }

          bulkRemovedItemsCount = args.OldItems.Count;
        }
      }

      ClassUnderTest.CollectionChanged += Handler;

      //Act
      ClassUnderTest.Clear();

      ClassUnderTest.CollectionChanged -= Handler;

      //Assert
      removedItems.Count.Should().Be(expectedItemsCount);
      bulkRemovedItemsCount.Should().Be(expectedItemsCount);
    }

    #endregion

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

    #region AddRange

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void AddRange(bool supportsRangeNotifications)
    {
      //Arrange
      ClassUnderTest.SupportsRangeNotifications = supportsRangeNotifications;
      var itemsToAddRange = Enumerable.Range(1, 3).Select(i => new TestViewModel(new TestModel {Id = i})).ToArray();

      //Act
      ClassUnderTest.AddRange(itemsToAddRange);

      //Assert
      ClassUnderTest.Should().Equal(itemsToAddRange);

      ClassUnderTest.Count.Should().Be(3);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void AddRange_NewItemsCollectionChangeNotification(bool supportsRangeNotifications)
    {
      //Arrange
      ClassUnderTest.SupportsRangeNotifications = supportsRangeNotifications;

      var itemsToAddRange = Enumerable.Range(1, 3).Select(i => new TestViewModel(new TestModel {Id = i})).ToArray();
      IList addedItems = new List<TestViewModel>();

      int addedItemsCount = 0;

      void Handler(object sender, NotifyCollectionChangedEventArgs args)
      {
        if (args.Action == NotifyCollectionChangedAction.Add)
          foreach (var newItem in args.NewItems)
            addedItems.Add(newItem);

        addedItemsCount = args.NewItems.Count;
      }

      ClassUnderTest.CollectionChanged += Handler;

      //Act
      ClassUnderTest.AddRange(itemsToAddRange);

      ClassUnderTest.CollectionChanged -= Handler;

      //Assert
      addedItems.Count.Should().Be(itemsToAddRange.Length);
      addedItemsCount.Should().Be(supportsRangeNotifications ? itemsToAddRange.Length : 1);
    }

    #endregion

    #region RemoveRange

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void RemoveRange(bool supportsRangeNotifications)
    {
      //Arrange
      ClassUnderTest.SupportsRangeNotifications = supportsRangeNotifications;

      AddIdRange(new Range(2, 5));
      var item1 = ClassUnderTest[0];
      var removeItemsRange = ClassUnderTest.Skip(1).ToList();

      //Act
      ClassUnderTest.RemoveRange(removeItemsRange);

      //Assert
      ClassUnderTest.Count.Should().Be(1);

      ClassUnderTest.Contains(item1).Should().BeTrue();
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void RemoveRange_OldItemsCollectionChangeNotification(bool supportsRangeNotifications)
    {
      //Arrange
      ClassUnderTest.SupportsRangeNotifications = supportsRangeNotifications;

      AddIdRange(new Range(2, 5));
      
      var removeItemsRange = ClassUnderTest.Skip(2).ToArray();

      IList removedItems = new List<TestViewModel>();
      int removedItemsCount = 0;

      void Handler(object sender, NotifyCollectionChangedEventArgs args)
      {
        if (args.Action == NotifyCollectionChangedAction.Remove)
        {
          foreach (var oldItem in args.OldItems)
          {
            removedItems.Add(oldItem);
          }

          removedItemsCount = args.OldItems.Count;
        }
      }

      ClassUnderTest.CollectionChanged += Handler;

      //Act
      ClassUnderTest.RemoveRange(removeItemsRange);

      ClassUnderTest.CollectionChanged -= Handler;

      //Assert
      removedItems.Count.Should().Be(removeItemsRange.Length);
      removedItemsCount.Should().Be(supportsRangeNotifications ? removeItemsRange.Length : 1);
    }

    #endregion
  }
}