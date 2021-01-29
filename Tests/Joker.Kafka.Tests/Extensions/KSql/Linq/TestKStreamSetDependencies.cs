using Kafka.DotNet.ksqlDB.KSql.Linq;
using Kafka.DotNet.ksqlDB.KSql.Query;
using Kafka.DotNet.ksqlDB.KSql.Query.Context;
using Kafka.DotNet.ksqlDB.KSql.RestApi;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Parameters;
using Kafka.DotNet.ksqlDB.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Linq
{
  public class TestKStreamSetDependencies : IKStreamSetDependencies
  {
    public TestKStreamSetDependencies()
    {
      KSqldbProviderMock = new Mock<IKSqlDbProvider>();

      KsqlDBProvider = KSqldbProviderMock.Object;

      KSqlDBContextOptions = new KSqlDBContextOptions(TestParameters.KsqlDBUrl);

      QueryContext = new QueryContext();
      KSqlQueryGenerator = new KSqlQueryGenerator(KSqlDBContextOptions);

      QueryStreamParameters = new QueryStreamParameters();

      var serviceCollection = new ServiceCollection();
      
      serviceCollection.AddSingleton<IKStreamSetDependencies>(this);
      
      var serviceProvider = serviceCollection.BuildServiceProvider(new ServiceProviderOptions {ValidateScopes = true});
      var serviceScopeFactory = serviceProvider.GetService<IServiceScopeFactory>();

      Provider = new QbservableProvider(serviceScopeFactory);
    }

    public IKSqlQbservableProvider Provider { get; }
    public IKSqlDbProvider KsqlDBProvider { get; set; }
    public IKSqlQueryGenerator KSqlQueryGenerator { get; set; }
    public QueryStreamParameters QueryStreamParameters { get; }
    
    public QueryContext QueryContext { get; }

    public KSqlDBContextOptions KSqlDBContextOptions { get; set; }

    public Mock<IKSqlDbProvider> KSqldbProviderMock { get; }
  }
}