using System;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;
using Moq;
using Moq.Protected;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Linq
{
  public class PeopleQueryStream : KQueryStreamSet<Person>
  {
    private readonly TestKStreamSetDependencies dependencies;

    public PeopleQueryStream(TestKStreamSetDependencies dependencies)
      : base(dependencies)
    {
      this.dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));

      dependencies.KSqlQueryGenerator = MockExtensions.CreateKSqlQueryGenerator("People");
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