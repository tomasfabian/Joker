using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Parameters;
using Kafka.DotNet.ksqlDB.Sample.Model;

namespace Kafka.DotNet.ksqlDB.Sample.QueryStreams
{
  public class TweetsQueryStream : KQueryStreamSet<Tweet>
  {
    public TweetsQueryStream(string ksqlDbUrl) 
      : base(new QbservableProvider(ksqlDbUrl))
    {
    }

    protected override QueryStreamParameters CreateQueryParameters(string ksqlQuery)
    {
      var queryParameters = base.CreateQueryParameters(ksqlQuery);

      queryParameters["auto.offset.reset"] = "earliest";

      return queryParameters;
    }
  }
}