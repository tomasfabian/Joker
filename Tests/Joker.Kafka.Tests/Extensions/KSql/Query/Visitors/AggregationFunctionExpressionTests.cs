using System;
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
      ClassUnderTest = new AggregationFunctionVisitor(StringBuilder);
    }

    [TestMethod]
    public void Sqrt_BuildKSql_PrintsSqrtWithColumn()
    {
      //Arrange
      Expression<Func<IKSqlGrouping<int, System.Drawing.Rectangle>, object>> expression = l => new { Key = l.Key, Sqrt = l.Sqrt(c => c.Height) };

      //Act
      var query = ClassUnderTest.BuildKSql(expression);

      //Assert
      query.Should().BeEquivalentTo($"Key, SQRT({nameof(System.Drawing.Rectangle.Height)}) Sqrt");
    }

    [TestMethod]
    public void Sign_BuildKSql_PrintsSignWithColumn()
    {
      //Arrange
      Expression<Func<IKSqlGrouping<int, Transaction>, object>> expression = l => new { Key = l.Key, Sign = l.Sign(c => c.Amount) };

      //Act
      var query = ClassUnderTest.BuildKSql(expression);

      //Assert
      query.Should().BeEquivalentTo($"Key, SIGN({nameof(Transaction.Amount)}) Sign");
    }
  }
}