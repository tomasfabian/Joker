using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentAssertions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Linq
{
  [TestClass]
  public class QbservableExtensionsTests : TestBase
  {
    private IQbservable<Location> CreateStreamSource()
    {
      return new KStreamSet<Location>(new QbservableProvider(@"http:\\localhost:8088"));
    }

    [TestMethod]
    public void ToQueryString_BuildKSql_PrintsQuery()
    {
      //Arrange
      int limit = 2;

      var query = CreateStreamSource()
        .Take(limit);

      //Act
      var ksql = query.ToQueryString();
      
      //Assert
      ksql.Should().BeEquivalentTo(@$"SELECT * FROM Locations EMIT CHANGES LIMIT {limit};");
    }

    #region Subscribe

    [TestMethod]
    public void Subscribe_BuildKSql_ObservesItems()
    {
      //Arrange
      var query = new TestableKStreamSet(new QbservableProvider(@"http:\\localhost:8088"));

      var results = new List<string>();

      //Act
      using var disposable = query.Subscribe(value =>
      {
        results.Add(value);
      });
      
      //Assert
      Assert.AreEqual(2, results.Count);
    }

    #endregion
  }
}