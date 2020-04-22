using System.ComponentModel;
using System.Linq;
using FluentAssertions;
using Joker.Collections;
using Joker.Extensions.Sorting;
using Joker.MVVM.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;

namespace Joker.MVVM.Tests.Joker.Extensions
{
  [TestClass]
  public class SortExtensionsTests : TestBase
  {
    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();
    }

    [TestMethod]
    public void CompareAB_NameDescending()
    {
      //Arrange
      var sortDescriptions = CreateSortDescriptions();

      //Act
      var comparer = sortDescriptions.ToComparer();

      //Assert
      comparer.Compare(new TestModel { Name = "a" }, new TestModel { Name = "b" }).Should().BePositive();
    }

    [TestMethod]
    public void CompareAA_NameDescending_Equal()
    {
      //Arrange
      var sortDescriptions = CreateSortDescriptions();

      //Act
      var comparer = sortDescriptions.ToComparer();

      //Assert
      comparer.Compare(new TestModel { Name = "a" }, new TestModel { Name = "a" }).Should().Be(0);
    }

    [TestMethod]
    public void CompareBA_NameDescending()
    {
      //Arrange
      var sortDescriptions = CreateSortDescriptions();

      //Act
      var comparer = sortDescriptions.ToComparer();

      //Assert
      comparer.Compare(new TestModel { Name = "b" }, new TestModel { Name = "a" }).Should().BeNegative();
    }

    private static Sort<TestModel>[] CreateSortDescriptions()
    {
      var sortByName = new Sort<TestModel>(c => c.Name, ListSortDirection.Descending);
      var sortById = new Sort<TestModel>(c => c.Id);

      var sortDescriptions = new[]
      {
        sortByName, sortById
      };

      return sortDescriptions;
    }

    [TestMethod]
    public void CompareA1A2_NameDescending()
    {
      //Arrange
      var sortDescriptions = CreateSortDescriptions();

      //Act
      var comparer = sortDescriptions.ToComparer();

      //Assert
      comparer.Compare(new TestModel { Name = "a", Id = 1}, new TestModel { Name = "a", Id = 2}).Should().BeNegative();
    }

    [TestMethod]
    public void CompareA1A1_NameDescending_Equal()
    {
      //Arrange
      var sortDescriptions = CreateSortDescriptions();

      //Act
      var comparer = sortDescriptions.ToComparer();

      //Assert
      comparer.Compare(new TestModel { Name = "a", Id = 1}, new TestModel { Name = "a", Id = 1}).Should().Be(0);
    }

    [TestMethod]
    public void CompareA2A1_NameDescending()
    {
      //Arrange
      var sortDescriptions = CreateSortDescriptions();

      //Act
      var comparer = sortDescriptions.ToComparer();

      //Assert
      comparer.Compare(new TestModel { Name = "a", Id = 2}, new TestModel { Name = "a", Id = 1}).Should().BePositive();
    }
  }
}