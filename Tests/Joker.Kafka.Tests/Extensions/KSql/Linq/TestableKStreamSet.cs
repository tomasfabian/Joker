using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi;
using Moq;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Linq
{
  internal abstract class TestableKStreamSet<TEntity> : KStreamSet<TEntity>
  {
    internal TestableKStreamSet(TestKStreamSetDependencies dependencies)
      : base(dependencies)
    {
      KSqldbProviderMock = dependencies.KSqldbProviderMock;
            
      KSqldbProviderMock.Setup(c => c.Run<TEntity>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
        .Callback<object, CancellationToken>((par, ct) => { CancellationToken = ct; })
        .Returns(GetTestValues);
    }

    protected TestableKStreamSet(IKStreamSetDependencies dependencies, Expression expression)
      : base(dependencies, expression)
    {
    }

    public Mock<IKSqldbProvider> KSqldbProviderMock { get; }

    public CancellationToken CancellationToken { get; private set; }

    protected abstract IAsyncEnumerable<TEntity> GetTestValues();
  }

  internal class TestableKStreamSet : TestableKStreamSet<string>
  {
    public TestableKStreamSet(TestKStreamSetDependencies dependencies) 
      : base(dependencies)
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