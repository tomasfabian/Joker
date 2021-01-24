using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;
using Moq;
using Moq.Protected;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Linq
{
  public class PeopleQueryStream : KQueryStreamSet<Person>
  {
    public PeopleQueryStream()
      : base(new TestKStreamSetDependencies())
    {
    }

    public override IKSqlQueryGenerator CreateKSqlQueryGenerator()
    {
      var queryGenerator = MockExtensions.CreateKSqlQueryGenerator("People");

      return queryGenerator;
    }
  }

  public static class MockExtensions
  {
    public static IKSqlQueryGenerator CreateKSqlQueryGenerator(string streamName)
    {
      var queryGenerator = new Mock<KSqlQueryGenerator>();
      
      queryGenerator.Protected()
        .Setup<string>(
          "InterceptStreamName",
          ItExpr.IsAny<string>()
        )
        .Returns(streamName)
        .Verifiable();

      return queryGenerator.Object;
    }
  }
}