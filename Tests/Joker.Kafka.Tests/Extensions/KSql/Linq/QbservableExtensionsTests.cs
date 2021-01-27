using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;
using Kafka.DotNet.ksqlDB.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnitTests;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Linq
{
  [TestClass]
  public class QbservableExtensionsTests : TestBase
  {
    private IQbservable<Location> CreateStreamSource()
    {
      return new KQueryStreamSet<Location>(new TestKStreamSetDependencies());
    }    
    
    private TestableKStreamSet CreateTestableKStreamSet()
    {
      return new TestableKStreamSet(new TestKStreamSetDependencies());
    }

    [TestMethod]
    public void SelectConstant_BuildKSql_PrintsConstant()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(c => "Hello world");

      //Act
      var ksql = query.ToQueryString();
      
      //Assert
      ksql.Should().BeEquivalentTo(@$"SELECT 'Hello world' FROM Locations EMIT CHANGES;");
    }

    [TestMethod]
    [Ignore("TODO")]
    public void SelectConstants_BuildKSql_PrintsConstants()
    {
      //Arrange
      var query = CreateStreamSource()
        .Select(c => new { Message = "Hello world", Age = 23 });

      //Act
      var ksql = query.ToQueryString();
      
      //Assert
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

    [TestMethod]
    public void ToQueryString_BuildKSqlOnDerivedClass_PrintsQuery()
    {
      //Arrange
      var query = new TweetsQueryStream();

      //Act
      var ksql = query.ToQueryString();
      
      //Assert
      ksql.Should().BeEquivalentTo(@$"SELECT * FROM Tweets EMIT CHANGES;");
    }

    [TestMethod]
    public async Task ToAsyncEnumerable_Query_KSqldbProviderRunWasCalled()
    {
      //Arrange
      var query = CreateTestableKStreamSet();

      //Act
      var asyncEnumerable = query.ToAsyncEnumerable();

      //Assert
      query.KSqldbProviderMock.Verify(c => c.Run<string>(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);

      await asyncEnumerable.GetAsyncEnumerator().DisposeAsync();
    }

    [TestMethod]
    public async Task ToAsyncEnumerable_Enumerate_ValuesWereReceived()
    {
      //Arrange
      var query = CreateTestableKStreamSet();

      //Act
      var asyncEnumerable = query.ToAsyncEnumerable();

      //Assert
      bool wasValueReceived = false;
      await foreach (var value in asyncEnumerable)
        wasValueReceived = true;

      wasValueReceived.Should().BeTrue();
    }

    [TestMethod]
    public void ToObservable_QueryShouldBeDeferred_KSqldbProviderRunWasNotCalled()
    {
      //Arrange
      var query = CreateTestableKStreamSet();

      //Act
      var observable = query.ToObservable();

      //Assert
      observable.Should().NotBeNull();
      
      query.KSqldbProviderMock.Verify(c => c.Run<string>(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public void ToObservable_DisposeSubscription()
    {
      //Arrange
      var query = CreateTestableKStreamSet();

      //Act
      query.ToObservable().Subscribe().Dispose();

      //Assert
      query.CancellationToken.IsCancellationRequested.Should().BeTrue();
    }

    [TestMethod]
    public void Subscribe_BuildKSql_ObservesItems()
    {
      //Arrange
      var query = CreateTestableKStreamSet();

      var results = new List<string>();

      //Act
      using var disposable = query.Subscribe(value =>
      {
        results.Add(value);
      }, error => { }, () => { });

      //Assert
      Assert.AreEqual(2, results.Count);
    }
  }
}