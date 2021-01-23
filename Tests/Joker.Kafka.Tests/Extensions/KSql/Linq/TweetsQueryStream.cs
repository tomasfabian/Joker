using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi;
using Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.RestApi;
using Moq;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Linq
{
  public class TweetsQueryStream : KQueryStreamSet<KSqlDbProviderTests.Tweet>
  {
    public TweetsQueryStream(string ksqlDbUrl)
      : base(new QbservableProvider(ksqlDbUrl))
    {
    }

    protected override IKSqldbProvider<KSqlDbProviderTests.Tweet> CreateKSqlDbProvider()
    {
      var httpClientMock = new Mock<IHttpClientFactory>();

      return new TestableKSqlDbQueryStreamProvider(httpClientMock.Object);
    }
  }
}