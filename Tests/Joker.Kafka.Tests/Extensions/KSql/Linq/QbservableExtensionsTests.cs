using System;
using System.Collections.Generic;
using FluentAssertions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;
using Kafka.DotNet.ksqlDB.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Linq
{
  [TestClass]
  public class QbservableExtensionsTests : TestBase
  {
    private IQbservable<Location> CreateStreamSource()
    {
      return new KQueryStreamSet<Location>(new QbservableProvider(@"http:\\localhost:8088"));
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
    [Ignore]
    public void Subscribe_BuildKSql_ObservesItems()
    {
      //Arrange
      var query = new TestableKStreamSet(new QbservableProvider(@"http:\\localhost:8088"));

      var results = new List<string>();

      //Act
      using var disposable = query.Subscribe(value =>
      {
        results.Add(value);
      }, error => { }, () => { });

      //Assert
      Assert.AreEqual(2, results.Count);
    }

    #endregion
  }
}