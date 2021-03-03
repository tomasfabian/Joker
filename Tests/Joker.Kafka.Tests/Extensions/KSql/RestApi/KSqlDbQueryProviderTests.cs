﻿using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Parameters;
using Kafka.DotNet.ksqlDB.Tests.Pocos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using UnitTests;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.RestApi
{
  [TestClass]
  public class KSqlDbQueryProviderTests : TestBase
  {
    private TestableKSqlDbQueryProvider ClassUnderTest { get; set; }

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      ClassUnderTest = MockingKernel.Get<TestableKSqlDbQueryProvider>();
    }

    [TestMethod]
    public async Task Run_HttpStatusCodeOK_ReturnsTweets()
    {
      //Arrange
      var queryParameters = new QueryStreamParameters();

      //Act
      var items = ClassUnderTest.Run<Nested>(queryParameters);

      //Assert
      var receivedTweets = new List<Nested>();
      await foreach (var item in items)
      {
        item.Should().NotBeNull();
        receivedTweets.Add(item);
      }

      receivedTweets.Count.Should().Be(2);
    }

    internal struct MovieStruct
    {
      public string Title { get; set; }

      public int Id { get; set; }
    }

    internal class Nested
    {
      public int Id { get; set; }
      public MovieStruct[] Arr { get; set; }
      public Dictionary<string, Dictionary<string, int>> MapValue { get; set; }
      public Dictionary<int, string[]> MapArr { get; set; }
      public MovieStruct Movie { get; set; }
      public int Release_Year { get; set; }
    }
  }
}