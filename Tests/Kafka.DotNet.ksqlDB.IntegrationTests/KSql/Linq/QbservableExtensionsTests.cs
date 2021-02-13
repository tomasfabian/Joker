﻿using System;
using System.Collections.Generic;
using System.Globalization;
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
      int itemsCount = 2;
      
      //Act
      var source = context.CreateQueryStream<Tweet>(streamName)
        .Take(itemsCount)
        .ToAsyncEnumerable();

      var actualValues = await CollectActualValues(source);

      var expectedValues = new List<Tweet>
      {
        Tweet1, Tweet2
      };

      //Assert
      Assert.AreEqual(itemsCount, actualValues.Count);
      CollectionAssert.AreEqual(expectedValues, actualValues);
    }

    private static async Task<List<Tweet>> CollectActualValues(IAsyncEnumerable<Tweet> source)
    {
      var actualValues = new List<Tweet>();

      var cts = new CancellationTokenSource();
      cts.CancelAfter(TimeSpan.FromSeconds(1.5));

      await foreach (var item in source.WithCancellation(cts.Token))
      {
        actualValues.Add(item);
      }

      return actualValues;
    }

    [TestMethod]
    public async Task Where_MessageWasFiltered()
    {
      //Arrange
      int itemsCount = 1;

      //Act
      var source = context.CreateQueryStream<Tweet>(streamName)
        .Where(p => p.Message != "Hello world")
        .Take(itemsCount)
        .ToAsyncEnumerable();

      var actualValues = await CollectActualValues(source);
      
      //Assert
      Assert.AreEqual(itemsCount, actualValues.Count);
      Assert.AreEqual(actualValues[0].Message, Tweet2.Message);
    }
  }
}