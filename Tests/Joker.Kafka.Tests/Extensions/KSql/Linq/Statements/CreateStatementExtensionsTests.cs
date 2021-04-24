using FluentAssertions;
using Kafka.DotNet.ksqlDB.KSql.Linq.Statements;
using Kafka.DotNet.ksqlDB.Tests.Helpers;
using Kafka.DotNet.ksqlDB.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Linq.Statements
{
  [TestClass]
  public class CreateStatementExtensionsTests : TestBase
    {
      private TestableDbProvider DbProvider { get; set; }

      [TestInitialize]
      public override void TestInitialize()
      {
        base.TestInitialize();

        DbProvider = new TestableDbProvider(TestParameters.KsqlDBUrl);
      }

      private const string StreamName = "TestStream";

      [TestMethod]
      public void CreateOrReplaceStreamStatement_ToStatementString_CalledTwiceWithSameResult()
      {
        //Arrange
        var query = DbProvider.CreateOrReplaceStreamStatement(StreamName)
          .As<Location>();

        //Act
        var ksql1 = query.ToStatementString();
        var ksql2 = query.ToStatementString();

        //Assert
        ksql1.Should().BeEquivalentTo(@$"CREATE OR REPLACE STREAM {StreamName}
AS SELECT * FROM {nameof(Location)}s EMIT CHANGES;");

        ksql1.Should().BeEquivalentTo(ksql2);
      }
    }
}