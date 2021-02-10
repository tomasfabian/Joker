﻿using System;
using System.Linq.Expressions;
using System.Text;
using FluentAssertions;
using Kafka.DotNet.ksqlDB.KSql.Linq;
using Kafka.DotNet.ksqlDB.KSql.Query.Visitors;
using Kafka.DotNet.ksqlDB.Tests.Pocos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Query.Visitors
{
  [TestClass]
  public class AggregationFunctionExpressionTests : TestBase
  {
    private AggregationFunctionVisitor ClassUnderTest { get; set; }
    private StringBuilder StringBuilder { get; set; }

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      StringBuilder = new StringBuilder();
      ClassUnderTest = new AggregationFunctionVisitor(StringBuilder, useTableAlias: false);
    }

    [TestMethod]
    public void LongCount_BuildKSql_PrintsCountWithAsterix()
    {
      //Arrange
      Expression<Func<IKSqlGrouping<int, Transaction>, object>> expression = l => new { Key = l.Key, Count = l.LongCount() };

      //Act
      var query = ClassUnderTest.BuildKSql(expression);

      //Assert
      query.Should().BeEquivalentTo($"Key, COUNT(*) Count");
    }

    [TestMethod]
    public void LongCount_BuildKSql_PrintsCountWithColumn()
    {
      //Arrange
      Expression<Func<IKSqlGrouping<int, Transaction>, object>> expression = l => new { Key = l.Key, Count = l.LongCount(c => c.Amount) };
      
      //Act
      var query = ClassUnderTest.BuildKSql(expression);

      //Assert
      query.Should().BeEquivalentTo($"Key, COUNT({nameof(Transaction.Amount)}) Count");
    }

    [TestMethod]
    public void Count_BuildKSql_PrintsCountWithColumn()
    {
      //Arrange
      Expression<Func<IKSqlGrouping<int, Transaction>, object>> expression = l => new { Key = l.Key, Count = l.Count(c => c.Amount) };

      //Act
      var query = ClassUnderTest.BuildKSql(expression);

      //Assert
      query.Should().BeEquivalentTo($"Key, COUNT({nameof(Transaction.Amount)}) Count");
    }

    [TestMethod]
    public void Max_BuildKSql_PrintsMaxWithColumn()
    {
      //Arrange
      Expression<Func<IKSqlGrouping<int, System.Drawing.Rectangle>, object>> expression = l => new { Key = l.Key, Max = l.Max(c => c.Height) };

      //Act
      var query = ClassUnderTest.BuildKSql(expression);

      //Assert
      query.Should().BeEquivalentTo($"Key, MAX({nameof(System.Drawing.Rectangle.Height)}) Max");
    }

    [TestMethod]
    public void Min_BuildKSql_PrintsMinWithColumn()
    {
      //Arrange
      Expression<Func<IKSqlGrouping<int, Transaction>, object>> expression = l => new { Key = l.Key, Min = l.Min(c => c.Amount) };

      //Act
      var query = ClassUnderTest.BuildKSql(expression);

      //Assert
      query.Should().BeEquivalentTo($"Key, MIN({nameof(Transaction.Amount)}) Min");
    }

    [TestMethod]
    public void EarliestByOffset_BuildKSql_PrintsEarliestByOffsetWithColumn()
    {
      //Arrange
      Expression<Func<IKSqlGrouping<int, Transaction>, object>> expression = l => new { Key = l.Key, EarliestByOffset = l.EarliestByOffset(c => c.Amount) };

      //Act
      var query = ClassUnderTest.BuildKSql(expression);

      //Assert
      query.Should().BeEquivalentTo($"Key, EARLIEST_BY_OFFSET({nameof(Transaction.Amount)}, true) EarliestByOffset");
    }

    [TestMethod]
    public void EarliestByOffsetN_BuildKSql_PrintsEarliestByOffsetWithColumn()
    {
      //Arrange
      Expression<Func<IKSqlGrouping<int, Transaction>, object>> expression = l => new { Key = l.Key, EarliestByOffset = l.EarliestByOffset(c => c.Amount, 2) };

      //Act
      var query = ClassUnderTest.BuildKSql(expression);

      //Assert
      query.Should().BeEquivalentTo($"Key, EARLIEST_BY_OFFSET({nameof(Transaction.Amount)}, 2, true) EarliestByOffset");
    }

    [TestMethod]
    public void EarliestByOffsetAllowNulls_BuildKSql_PrintsEarliestByOffsetAllowNullsWithColumn()
    {
      //Arrange
      Expression<Func<IKSqlGrouping<int, Transaction>, object>> expression = l => new { Key = l.Key, EarliestByOffsetAllowNulls = l.EarliestByOffsetAllowNulls(c => c.Amount) };

      //Act
      var query = ClassUnderTest.BuildKSql(expression);

      //Assert
      query.Should().BeEquivalentTo($"Key, EARLIEST_BY_OFFSET({nameof(Transaction.Amount)}, false) EarliestByOffsetAllowNulls");
    }

    [TestMethod]
    public void EarliestByOffsetAllowNullsN_BuildKSql_PrintsEarliestByOffsetAllowNullsWithColumn()
    {
      //Arrange
      int earliestN = 3;
      Expression<Func<IKSqlGrouping<int, Transaction>, object>> expression = l => new { Key = l.Key, EarliestByOffsetAllowNulls = l.EarliestByOffsetAllowNulls(c => c.Amount, earliestN) };

      //Act
      var query = ClassUnderTest.BuildKSql(expression);

      //Assert
      query.Should().BeEquivalentTo($"Key, EARLIEST_BY_OFFSET({nameof(Transaction.Amount)}, {earliestN}, false) EarliestByOffsetAllowNulls");
    }

    [TestMethod]
    public void LatestByOffset_BuildKSql_PrintsLatestByOffsetWithColumn()
    {
      //Arrange
      Expression<Func<IKSqlGrouping<int, Transaction>, object>> expression = l => new { Key = l.Key, LatestByOffset = l.LatestByOffset(c => c.Amount) };

      //Act
      var query = ClassUnderTest.BuildKSql(expression);

      //Assert
      query.Should().BeEquivalentTo($"Key, LATEST_BY_OFFSET({nameof(Transaction.Amount)}, true) LatestByOffset");
    }

    [TestMethod]
    public void LatestByOffsetN_BuildKSql_PrintsLatestByOffsetWithColumn()
    {
      //Arrange
      int latestN = 3;
      Expression<Func<IKSqlGrouping<int, Transaction>, object>> expression = l => new { Key = l.Key, LatestByOffset = l.LatestByOffset(c => c.Amount, latestN) };

      //Act
      var query = ClassUnderTest.BuildKSql(expression);

      //Assert
      query.Should().BeEquivalentTo($"Key, LATEST_BY_OFFSET({nameof(Transaction.Amount)}, {latestN}, true) LatestByOffset");
    }

    [TestMethod]
    public void LatestByOffsetAllowNulls_BuildKSql_PrintsLatestByOffsetAllowNullsWithColumn()
    {
      //Arrange
      Expression<Func<IKSqlGrouping<int, Transaction>, object>> expression = l => new { Key = l.Key, LatestByOffsetAllowNulls = l.LatestByOffsetAllowNulls(c => c.Amount) };
      
      //Act
      var query = ClassUnderTest.BuildKSql(expression);

      //Assert
      query.Should().BeEquivalentTo($"Key, LATEST_BY_OFFSET({nameof(Transaction.Amount)}, false) LatestByOffsetAllowNulls");
    }

    [TestMethod]
    public void LatestByOffsetAllowNullsN_BuildKSql_PrintsLatestByOffsetAllowNullsWithColumn()
    {
      //Arrange
      Expression<Func<IKSqlGrouping<int, Transaction>, object>> expression = l => new { Key = l.Key, LatestByOffsetAllowNulls = l.LatestByOffsetAllowNulls(c => c.Amount, 2) };
      
      //Act
      var query = ClassUnderTest.BuildKSql(expression);

      //Assert
      query.Should().BeEquivalentTo($"Key, LATEST_BY_OFFSET({nameof(Transaction.Amount)}, 2, false) LatestByOffsetAllowNulls");
    }

    [TestMethod]
    public void TopK_BuildKSql_PrintsTopK()
    {
      //Arrange
      Expression<Func<IKSqlGrouping<int, Transaction>, object>> expression = l => new { Key = l.Key, TopK = l.TopK(c => c.Amount, 2) };
      
      //Act
      var query = ClassUnderTest.BuildKSql(expression);

      //Assert
      query.Should().BeEquivalentTo($"Key, TOPK({nameof(Transaction.Amount)}, 2) TopK");
    }

    [TestMethod]
    public void TopKDistinct_BuildKSql_PrintsTopKDistinct()
    {
      //Arrange
      Expression<Func<IKSqlGrouping<int, Transaction>, object>> expression = l => new { Key = l.Key, TopKDistinct = l.TopKDistinct(c => c.Amount, 2) };
      
      //Act
      var query = ClassUnderTest.BuildKSql(expression);

      //Assert
      query.Should().BeEquivalentTo($"Key, TOPKDISTINCT({nameof(Transaction.Amount)}, 2) TopKDistinct");
    }

    [TestMethod]
    public void TopKDistinctWithVariableInput_BuildKSql_PrintsTopKDistinct()
    {
      //Arrange
      int k = 2;
      Expression<Func<IKSqlGrouping<int, Transaction>, object>> expression = l => new { Key = l.Key, TopKDistinct = l.TopKDistinct(c => c.Amount, k) };

      //Act
      var query = ClassUnderTest.BuildKSql(expression);

      //Assert
      query.Should().BeEquivalentTo($"Key, TOPKDISTINCT({nameof(Transaction.Amount)}, {k}) TopKDistinct");
    }
  }
}