using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi;
using Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.RestApi;
using Moq;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Linq
{
  internal class TweetsTestKStreamSetDependencies : TestKStreamSetDependencies
  {
    public TweetsTestKStreamSetDependencies()
    {
      var httpClientMock = new Mock<IHttpClientFactory>();

      KsqlDBProvider = new TestableKSqlDbQueryStreamProvider(httpClientMock.Object);
    }
  }
  public class TweetsQueryStream : KQueryStreamSet<KSqlDbProviderTests.Tweet>
  {
    public TweetsQueryStream()
      : base(new TweetsTestKStreamSetDependencies())
    {
    }
  }
}