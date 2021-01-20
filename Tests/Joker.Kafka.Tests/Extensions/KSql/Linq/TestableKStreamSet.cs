using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Parameters;
using Moq;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Linq
{
  internal class TestableKStreamSet : KStreamSet<string>
  {
    public TestableKStreamSet(IKSqlQbservableProvider provider) 
      : base(provider)
    {
    }

    protected override IKSqldbProvider<string> CreateKSqlDbProvider()
    {
      var mock = new Mock<IKSqldbProvider<string>>();

      mock.Setup(c => c.Run(It.IsAny<object>(), It.IsAny<CancellationToken>()))
        .Returns(GetTestValues);

      return mock.Object;
    }

    protected override object CreateQueryParameters(string ksqlQuery)
    {
      var queryParameters = new KsqlQueryParameters
      {
        KSql = ksqlQuery,
        ["ksql.streams.auto.offset.reset"] = "earliest"
      };

      return queryParameters;
    }

    public static async IAsyncEnumerable<string> GetTestValues()
    {
      yield return "Hello world";

      yield return "Goodbye";

      await Task.CompletedTask;
    }

    public static IAsyncEnumerable<string> GetAsyncRange(int count)
    {
       return Enumerable.Range(0, count).Select(c => c.ToString()).ToAsyncEnumerable();
    }
    
    static async IAsyncEnumerable<string> GetAsyncDelayed()
    {
      for (int i = 1; i <= 2; i++)
      {
        await Task.Delay(10); 
        yield return i.ToString();
      }
    }
  }
}