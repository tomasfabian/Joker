﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query.Context;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Parameters;
using Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Query.Context;
using Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.RestApi;
using Kafka.DotNet.ksqlDB.Tests.Helpers;
using Kafka.DotNet.ksqlDB.Tests.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnitTests;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Linq
{
  [TestClass]
  public class QbservableExtensionsTests : TestBase
  {
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

    public class TweetsKQueryStreamSet : KQueryStreamSet<KSqlDbProviderTests.Tweet>
    {
      public TweetsKQueryStreamSet(IServiceScopeFactory serviceScopeFactory, QueryContext queryContext) : base(serviceScopeFactory, queryContext)
      {
      }
    }

    public class TestableDbProviderExt : TestableDbProvider<KSqlDbProviderTests.Tweet>
    {
      public TestableDbProviderExt(string ksqlDbUrl) : base(ksqlDbUrl)
      {
      }

      public IQbservable<KSqlDbProviderTests.Tweet> CreateTweetsStreamSet(string streamName = null)
      {
        var serviceScopeFactory = Initialize();

        var queryStreamContext = new QueryContext
        {
          StreamName = streamName
        };

        return new TweetsKQueryStreamSet(serviceScopeFactory, queryStreamContext);
      }    
      
      protected override void OnConfigureServices(IServiceCollection serviceCollection)
      {
        serviceCollection.AddSingleton(KSqldbProviderMock.Object);
      }
    }

    [TestMethod]
    public void ToQueryString_BuildKSqlOnDerivedClass_PrintsQuery()
    {
      //Arrange
      var context = new TestableDbProviderExt(TestParameters.KsqlDBUrl);
      var query = context.CreateTweetsStreamSet();

      //Act
      var ksql = query.ToQueryString();

      //Assert
      ksql.Should().BeEquivalentTo(@$"SELECT * FROM Tweets EMIT CHANGES;");
    }

    [TestMethod]
    public async Task ToAsyncEnumerable_Query_KSqldbProviderRunWasCalled()
    {
      //Arrange
      var context = new TestableDbProvider(TestParameters.KsqlDBUrl);
      context.KSqldbProviderMock.Setup(c => c.Run<string>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
        .Returns(GetTestValues);
      var query = context.CreateStreamSet<string>();

      //Act
      var asyncEnumerable = query.ToAsyncEnumerable();

      //Assert
      context.KSqldbProviderMock.Verify(c => c.Run<string>(It.IsAny<QueryStreamParameters>(), It.IsAny<CancellationToken>()), Times.Once);

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
      var context = new TestableDbProvider(TestParameters.KsqlDBUrl);
      
      var query = context.CreateStreamSet<string>();

      //Act
      var observable = query.ToObservable();

      //Assert
      observable.Should().NotBeNull();

      context.KSqldbProviderMock.Verify(c => c.Run<string>(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public void ToObservable_DisposeSubscription()
    {
      //Arrange
      CancellationToken cancellationToken = default;

      var context = new TestableDbProvider(TestParameters.KsqlDBUrl);
      context.KSqldbProviderMock.Setup(c => c.Run<string>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
        .Callback<object, CancellationToken>((par, ct) => { cancellationToken = ct; })
        .Returns(GetTestValues);

      var query = context.CreateStreamSet<string>();

      //Act
      query.ToObservable().Subscribe().Dispose();

      //Assert
      cancellationToken.IsCancellationRequested.Should().BeTrue();
    }

    protected async IAsyncEnumerable<string> GetTestValues()
    {
      yield return "Hello world";

      yield return "Goodbye";

      await Task.CompletedTask;
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

    private IQbservable<Location> CreateStreamSource()
    {
      var context = new TestableDbProvider(TestParameters.KsqlDBUrl);
      
      return context.CreateStreamSet<Location>();
    }

    private IQbservable<string> CreateTestableKStreamSet()
    {
      var context = new TestableDbProvider(TestParameters.KsqlDBUrl);
      
      context.KSqldbProviderMock.Setup(c => c.Run<string>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
        .Returns(GetTestValues);
      
      return context.CreateStreamSet<string>();
    }
  }
}