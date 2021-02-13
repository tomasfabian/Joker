using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kafka.DotNet.ksqlDB.IntegrationTests.KSql.RestApi;
using Kafka.DotNet.ksqlDB.KSql.Linq;
using Kafka.DotNet.ksqlDB.KSql.Query.Context;
using Kafka.DotNet.ksqlDB.KSql.RestApi;
using Kafka.DotNet.ksqlDB.Tests.Pocos;
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
    
    [ClassInitialize]
    public static async Task ClassInitialize(TestContext context)
    {
      var uri = new Uri(KSqlDbRestApiProvider.KsqlDbUrl);

      restApiProvider = new KSqlDbRestApiProvider(new HttpClientFactory(uri));

      var ksql = $"CREATE STREAM {streamName}(id INT, message VARCHAR, isRobot BOOLEAN, amount DOUBLE, accountBalance DECIMAL(16,4))\r\n  WITH (kafka_topic='{topicName}', value_format='json', partitions=1);";
      var result = await restApiProvider.ExecuteStatementAsync(ksql);

      var insert =
        $"INSERT INTO {streamName} (id, message, isRobot, amount, accountBalance) VALUES (1, 'Hello world', true, .42E-3, 1.2);";
      
      result = await restApiProvider.ExecuteStatementAsync(insert);

      insert = 
        $"INSERT INTO {streamName} (id, message, isRobot, amount, accountBalance) VALUES (2, 'Wall-e', false, 1E0, 1.2);";

      result = await restApiProvider.ExecuteStatementAsync(insert);
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
      var dropStream = $"DROP STREAM IF EXISTS {streamName} DELETE TOPIC;";
      
      var result = await restApiProvider.ExecuteStatementAsync(dropStream);
    }

    [TestMethod]
    public async Task Select()
    {
      await using var context = new KSqlDBContext(contextOptions);

      int itemsCount = 2;

      var cts = new CancellationTokenSource();
      cts.CancelAfter(TimeSpan.FromSeconds(1.5));

      var source = context.CreateQueryStream<Tweet>(streamName)
        .Take(itemsCount)
        .ToAsyncEnumerable();

      var actualValues = new List<Tweet>();

      await foreach (var item in source.WithCancellation(cts.Token))
      {
        actualValues.Add(item);
      }

      var expectedValues = new List<Tweet>
      {

      };

      Assert.AreEqual(itemsCount, actualValues.Count);
      //CollectionAssert.AreEqual(expectedValues, actualValues);
    }

    [TestMethod]
    public async Task Where()
    {
      await using var context = new KSqlDBContext(contextOptions);

      int itemsCount = 1;

      var source = context.CreateQueryStream<Tweet>(streamName)
        .Where(p => p.Message != "Hello world")
        .Take(itemsCount)
        .ToAsyncEnumerable();

      var actualValues = new List<Tweet>();
      
      await foreach (var item in source)
      {
        actualValues.Add(item);
      }

      Assert.AreEqual(itemsCount, actualValues.Count);
      Assert.AreEqual(actualValues[0].Message, "Wall-e");
    }
  }
}