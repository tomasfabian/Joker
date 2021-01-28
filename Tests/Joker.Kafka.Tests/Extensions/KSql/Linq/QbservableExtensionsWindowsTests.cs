﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query.Windows;
using Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Query.Context;
using Kafka.DotNet.ksqlDB.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnitTests;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Linq
{
  [TestClass]
  public class QbservableExtensionsWindowsTests : TestBase
  {
    [TestMethod]
    public void GroupByAndCount_BuildKSql_PrintsQueryWithTumblingWindow()
    {
      //Arrange
      var context = new TransactionsDbProvider(TestParameters.KsqlDBUrl);

      var grouping = context.CreateStreamSet<Transaction>()
        .GroupBy(c => c.CardNumber)
        .WindowedBy(new TimeWindows(Duration.OfSeconds(5)))
        .Select(g => new { CardNumber = g.Key, Count = g.Count() });

      //Act
      var ksql = grouping.ToQueryString();

      //Assert
      ksql.Should().BeEquivalentTo("SELECT CardNumber, COUNT(*) Count FROM Transactions WINDOW TUMBLING (SIZE 5 SECONDS) GROUP BY CardNumber EMIT CHANGES;");
    }

    [TestMethod]
    public void GroupByAndCount_BuildKSql_PrintsQueryWithTumblingWindowAndGracePeriod()
    {
      //Arrange
      var context = new TransactionsDbProvider(TestParameters.KsqlDBUrl);

      var grouping = context.CreateStreamSet<Transaction>()
        .GroupBy(c => c.CardNumber)
        .WindowedBy(new TimeWindows(Duration.OfSeconds(5)).WithGracePeriod(Duration.OfHours(2)))
        .Select(g => new { CardNumber = g.Key, Count = g.Count() });

      //Act
      var ksql = grouping.ToQueryString();

      //Assert
      ksql.Should().BeEquivalentTo("SELECT CardNumber, COUNT(*) Count FROM Transactions WINDOW TUMBLING (SIZE 5 SECONDS, GRACE PERIOD 2 HOURS) GROUP BY CardNumber EMIT CHANGES;");
    }

    [TestMethod]
    public void QueriesFromSameCreateStreamSetShouldNotAffectEachOther()
    {
      //Arrange
      var context = new TransactionsDbProvider(TestParameters.KsqlDBUrl);

      var grouping1 = context.CreateStreamSet<Transaction>("authorization_attempts_1")
        .GroupBy(c => c.CardNumber)
        .Select(g => new { CardNumber = g.Key, Count = g.Count() });

      var grouping2 = context.CreateStreamSet<Transaction>("authorization_attempts_2")
        .GroupBy(c => c.CardNumber)
        .Select(g => new { CardNumber = g.Key, Count = g.Count() });

      var take = grouping2.Take(2);

      //Act
      var ksql1 = grouping1.ToQueryString();
      var ksql2 = grouping2.ToQueryString();
      var ksqlTake = take.ToQueryString();

      //Assert
      ksql1.Should().BeEquivalentTo("SELECT CardNumber, COUNT(*) Count FROM authorization_attempts_1 GROUP BY CardNumber EMIT CHANGES;");
      ksql2.Should().BeEquivalentTo("SELECT CardNumber, COUNT(*) Count FROM authorization_attempts_2 GROUP BY CardNumber EMIT CHANGES;");
      ksqlTake.Should().BeEquivalentTo("SELECT CardNumber, COUNT(*) Count FROM authorization_attempts_2 GROUP BY CardNumber EMIT CHANGES LIMIT 2;");
    }

    internal class Transaction : Record
    {
      public string CardNumber { get; set; }
    }

    class TransactionsDbProvider : TestableDbProvider<Transaction>
    {
      protected override void OnConfigureServices(IServiceCollection serviceCollection)
      {
        serviceCollection.AddSingleton(KSqldbProviderMock.Object);
      }

      public TransactionsDbProvider(string ksqlDbUrl) : base(ksqlDbUrl)
      {
      }
    }
  }
}