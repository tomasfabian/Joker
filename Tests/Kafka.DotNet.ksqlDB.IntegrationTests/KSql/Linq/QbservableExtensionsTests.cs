using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Kafka.DotNet.ksqlDB.IntegrationTests.KSql.RestApi;
using Kafka.DotNet.ksqlDB.IntegrationTests.Models;
using Kafka.DotNet.ksqlDB.KSql.Linq;
using Kafka.DotNet.ksqlDB.KSql.Query.Context;
using Kafka.DotNet.ksqlDB.KSql.RestApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;

namespace Kafka.DotNet.ksqlDB.IntegrationTests.KSql.Linq
{
  [TestClass]
  [TestCategory("Integration")]
  public class QbservableExtensionsTests : TestBase
  {
    private KSqlDBContextOptions contextOptions;
    private static string streamName = "tweetsTest";
    private static string topicName = "tweetsTestTopic";

    private static KSqlDbRestApiProvider restApiProvider;

    private static readonly Tweet Tweet1 = new()
    {
      Id = 1,
      Message = "Hello world",
      IsRobot = true,
      Amount = 0.00042, 
      AccountBalance = 1.2M,
    };

    private static readonly Tweet Tweet2 = new()
    {
      Id = 2,
      Message = "Wall-e",
      IsRobot = false,
      Amount = 1, 
      AccountBalance = -5.6M,
    };

    [ClassInitialize]
    public static async Task ClassInitialize(TestContext context)
    {
      var uri = new Uri(KSqlDbRestApiProvider.KsqlDbUrl);

      restApiProvider = new KSqlDbRestApiProvider(new HttpClientFactory(uri));

      var ksql = $"CREATE STREAM {streamName}(id INT, message VARCHAR, isRobot BOOLEAN, amount DOUBLE, accountBalance DECIMAL(16,4))\r\n  WITH (kafka_topic='{topicName}', value_format='json', partitions=1);";
      var result = await restApiProvider.ExecuteStatementAsync(ksql);
      result.Should().BeTrue();
      
      var insert = CreateInsertTweetStatement(Tweet1);

      result = await restApiProvider.ExecuteStatementAsync(insert);
      result.Should().BeTrue();

      insert = CreateInsertTweetStatement(Tweet2);

      result = await restApiProvider.ExecuteStatementAsync(insert);      
      result.Should().BeTrue();
    }

    private static string CreateInsertTweetStatement(Tweet tweet)
    {
      var amount = tweet.Amount.ToString("E1", CultureInfo.InvariantCulture);

      string insert =
        $"INSERT INTO {streamName} (id, message, isRobot, amount, accountBalance) VALUES ({tweet.Id}, '{tweet.Message}', {tweet.IsRobot}, {amount}, {tweet.AccountBalance});";
      
      return insert;
    }

    private KSqlDBContext context;

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      contextOptions = new KSqlDBContextOptions(KSqlDbRestApiProvider.KsqlDbUrl);
      
      context = new KSqlDBContext(contextOptions);
    }

    [TestCleanup]
    public override void TestCleanup()
    {
      context.DisposeAsync().GetAwaiter().GetResult();

      base.TestCleanup();
    }

    [ClassCleanup]
    public static async Task ClassCleanup()
    {
      var result = await restApiProvider.DropStreamAndTopic(streamName);
    }

    [TestMethod]
    public async Task Select()
    {
      //Arrange
      int expectedItemsCount = 2;
      
      var source = context.CreateQueryStream<Tweet>(streamName)
        .ToAsyncEnumerable();
      
      //Act
      var actualValues = await CollectActualValues(source, expectedItemsCount);
      
      //Assert
      var expectedValues = new List<Tweet>
      {
        Tweet1, Tweet2
      };
      
      Assert.AreEqual(expectedItemsCount, actualValues.Count);
      CollectionAssert.AreEqual(expectedValues, actualValues);
    }

    [TestMethod]
    public async Task Take()
    {
      //Arrange
      int expectedItemsCount = 1;
      
      var source = context.CreateQueryStream<Tweet>(streamName)
        .Take(expectedItemsCount)
        .ToAsyncEnumerable();
      
      //Act
      var actualValues = await CollectActualValues(source, expectedItemsCount);
      
      //Assert
      var expectedValues = new List<Tweet>
      {
        Tweet1
      };

      Assert.AreEqual(expectedItemsCount, actualValues.Count);
      CollectionAssert.AreEqual(expectedValues, actualValues);
    }

    [TestMethod]
    public async Task Where_MessageWasFiltered()
    {
      //Arrange
      int expectedItemsCount = 1;
      
      var source = context.CreateQueryStream<Tweet>(streamName)
        .Where(p => p.Message != "Hello world")
        .ToAsyncEnumerable();
      
      //Act
      var actualValues = await CollectActualValues(source, expectedItemsCount);
      
      //Assert
      Assert.AreEqual(expectedItemsCount, actualValues.Count);
      Assert.AreEqual(actualValues[0].Message, Tweet2.Message);
    }

    [TestMethod]
    public async Task Subscribe()
    {
      //Arrange
      var semaphore = new SemaphoreSlim(initialCount: 0, 1);
      var actualValues = new List<Tweet>();

      int expectedItemsCount = 2;
      
      var source = context.CreateQueryStream<Tweet>(streamName);

      //Act
      using var subscription = source.Take(expectedItemsCount).Subscribe(c => actualValues.Add(c), e => semaphore.Release(), () => semaphore.Release());
      await semaphore.WaitAsync(TimeSpan.FromSeconds(4));

      //Assert
      Assert.AreEqual(expectedItemsCount, actualValues.Count);
    }

    private static async Task<List<Tweet>> CollectActualValues(IAsyncEnumerable<Tweet> source, int? expectedItemsCount = null)
    {
      var actualValues = new List<Tweet>();

      var cts = new CancellationTokenSource();
      cts.CancelAfter(TimeSpan.FromSeconds(1.5));

      if (expectedItemsCount.HasValue)
        source = source.Take(expectedItemsCount.Value);

      await foreach (var item in source.WithCancellation(cts.Token))
      {
        actualValues.Add(item);
      }

      return actualValues;
    }
  }
}