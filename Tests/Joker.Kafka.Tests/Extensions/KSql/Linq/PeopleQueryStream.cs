using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi;
using Moq;
using Moq.Protected;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Linq
{
  public class PeopleQueryStream : KQueryStreamSet<Person>
  {
    public PeopleQueryStream(string ksqlDbUrl)
      : base(new QbservableProvider(ksqlDbUrl))
    {
    }

    protected override IKSqldbProvider<Person> CreateKSqlDbProvider()
    {
      var dbProvider = new Mock<IKSqldbProvider<Person>>();

      return dbProvider.Object;
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