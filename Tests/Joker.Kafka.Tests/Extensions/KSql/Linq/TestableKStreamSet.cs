using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Parameters;
using Moq;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Linq
{
  internal abstract class TestableKStreamSet<TEntity> : KStreamSet<TEntity>
  {
    protected TestableKStreamSet(IKSqlQbservableProvider provider)
      : base(provider)
    {
      KSqldbProviderMock = new Mock<IKSqldbProvider<TEntity>>();
    }

    protected TestableKStreamSet(IKSqlQbservableProvider provider, Expression expression)
      : base(provider, expression)
    {
    }

    public Mock<IKSqldbProvider<TEntity>> KSqldbProviderMock { get; }

    public CancellationToken CancellationToken { get; private set; }

    protected override IKSqldbProvider<TEntity> CreateKSqlDbProvider()
    {
      KSqldbProviderMock.Setup(c => c.Run(It.IsAny<object>(), It.IsAny<CancellationToken>()))
        .Callback<object, CancellationToken>((par, ct) => { CancellationToken = ct; })
        .Returns(GetTestValues);

      return KSqldbProviderMock.Object;
    }

    protected abstract IAsyncEnumerable<TEntity> GetTestValues();

    protected override object CreateQueryParameters(string ksqlQuery)
    {
      var queryParameters = new KsqlQueryParameters
      {
        KSql = ksqlQuery,
        ["ksql.streams.auto.offset.reset"] = "earliest"
      };

      return queryParameters;
    }
  }

  internal class TestableKStreamSet : TestableKStreamSet<string>
  {
    public TestableKStreamSet(IKSqlQbservableProvider provider) 
      : base(provider)
    {
    }

    protected override async IAsyncEnumerable<string> GetTestValues()
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