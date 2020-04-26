using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Joker.OData.Extensions.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Joker.MVVM.Tests.OData
{
  [TestClass]
  public class ODataKeyTests
  {
    [Microsoft.OData.Client.Key(nameof(Key1), nameof(Key2))]
    public class ODataEntity
    {
      public string Key1 { get; set; }
      public int Key2 { get; set; }

      public double Value { get; set; }
    }

    [TestMethod]
    public void GetKeys()
    {
      //Arrange
      string key1 = "test";
      int key2 = 2;
      var predicate = ExpressionExtensions.CreatePredicate<ODataEntity>(key1, key2);

      //Act
      var hasSameKeys = predicate.Compile()(new ODataEntity { Key1 = key1, Key2 = key2 });
        
      //Assert
      hasSameKeys.Should().BeTrue();
    }
  }
}