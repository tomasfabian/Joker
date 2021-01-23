using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Exceptions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using UnitTests;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.RestApi
{
  [TestClass]
  public class KSqlDbProviderTests: TestBase<TestableKSqlDbQueryStreamProvider>
  {  
    public class Tweet
    {
      public int Id { get; set; }

      [JsonPropertyName("MESSAGE")]
      public string Message { get; set; }
    
      public bool IsRobot { get; set; }

      public double Amount { get; set; }
    }

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      ClassUnderTest = MockingKernel.Get<TestableKSqlDbQueryStreamProvider>();
    }

    [TestMethod]
    public async Task Run_HttpStatusCodeOK_ReturnsTweets()
    {
      //Arrange
      var queryParameters = new KsqlQueryParameters();

      //Act
      var tweets = ClassUnderTest.Run(queryParameters);

      //Assert
      var receivedTweets = new List<Tweet>();
      await foreach (var tweet in tweets)
      {
        tweet.Should().NotBeNull();
        receivedTweets.Add(tweet);
      }

      receivedTweets.Count.Should().Be(2);
    }

    [TestMethod]
    public async Task Run_HttpStatusCodeOK_StringFieldWasParsed()
    {
      //Arrange
      var queryParameters = new KsqlQueryParameters();

      //Act
      var tweets = await ClassUnderTest.Run(queryParameters).ToListAsync();

      //Assert
      var tweet = tweets[0];

      tweet.Message.Should().Be("Hello world");
    }

    [TestMethod]
    public async Task Run_HttpStatusCodeOK_BooleanFieldWasParsed()
    {
      //Arrange
      var queryParameters = new KsqlQueryParameters();

      //Act
      var tweets = await ClassUnderTest.Run(queryParameters).ToListAsync();

      //Assert
      var tweet = tweets[0];

      tweet.IsRobot.Should().BeTrue();
    }

    [TestMethod]
    public async Task Run_HttpStatusCodeOK_DoubleFieldWasParsed()
    {
      //Arrange
      var queryParameters = new KsqlQueryParameters();

      //Act
      var tweets = await ClassUnderTest.Run(queryParameters).ToListAsync();

      //Assert
      var tweet = tweets[0];

      tweet.Amount.Should().Be(0.1);
    }

    [TestMethod]
    public async Task Run_HttpStatusCodeOK_IntegerFieldWasParsed()
    {
      //Arrange
      var queryParameters = new KsqlQueryParameters();

      //Act
      var tweets = await ClassUnderTest.Run(queryParameters).ToListAsync();

      //Assert
      var tweet = tweets[0];

      tweet.Id.Should().Be(1);
    }

    [TestMethod]
    [ExpectedException(typeof(KSqlQueryException))]
    public async Task Run_HttpStatusCodeBadRequest_ThrowsException()
    {
      //Arrange
      ClassUnderTest.ShouldThrowException = true;

      var queryParameters = new KsqlQueryParameters();

      //Act
      var tweets = ClassUnderTest.Run(queryParameters);

      //Assert
      await foreach (var tweet in tweets)
      {
        tweet.Should().NotBeNull();
      }
    }

    [TestMethod]
    public async Task Run_Disposed_NothingWasReceived()
    {
      //Arrange
      var queryParameters = new KsqlQueryParameters();
      var cts = new CancellationTokenSource();

      //Act
      IAsyncEnumerable<Tweet> tweets = ClassUnderTest.Run(queryParameters, cts.Token);
      cts.Cancel();

      //Assert
      var receivedTweets = new List<Tweet>();

      await foreach (var tweet in tweets.WithCancellation(cts.Token))
      {
        receivedTweets.Add(tweet);
      }

      receivedTweets.Should().BeEmpty();
      cts.Dispose();
    }
  }
}