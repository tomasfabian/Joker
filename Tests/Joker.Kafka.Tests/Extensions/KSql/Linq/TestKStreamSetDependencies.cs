﻿using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi.Parameters;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Linq
{
  public class TestKStreamSetDependencies : IKStreamSetDependencies
  {
    public TestKStreamSetDependencies()
    {
      KSqldbProviderMock = new Mock<IKSqldbProvider>();

      KsqlDBProvider = KSqldbProviderMock.Object;

      QueryStreamParameters = new QueryStreamParameters();

      var serviceCollection = new ServiceCollection();

      serviceCollection.AddTransient<IKStreamSetDependencies, TestKStreamSetDependencies>();
      
      var serviceProvider = serviceCollection.BuildServiceProvider(new ServiceProviderOptions {ValidateScopes = true});
      Provider = new QbservableProvider(serviceProvider);
    }

    public IKSqlQbservableProvider Provider { get; }
    public IKSqldbProvider KsqlDBProvider { get; set; }
    public QueryStreamParameters QueryStreamParameters { get; }

    public Mock<IKSqldbProvider> KSqldbProviderMock { get; }
  }
}