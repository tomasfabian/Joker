using FluentAssertions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Linq
{
  [TestClass]
  public class InterceptStreamNameTests : TestBase
  {
    [TestMethod]
    public void InterceptStreamName()
    {
      //Arrange
      var query = new PeopleQueryStream(TestParameters.KsqlDBUrl);

      //Act
      var ksql = query.ToQueryString();

      //Assert
      ksql.Should().BeEquivalentTo("SELECT * FROM People EMIT CHANGES;");
    }
  }
}