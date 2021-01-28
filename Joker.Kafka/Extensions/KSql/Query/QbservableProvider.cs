using System;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query.Context;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  public class QbservableProvider : IKSqlQbservableProvider
  {
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly QueryContext queryContext;

    public QbservableProvider(IServiceScopeFactory serviceScopeFactory, QueryContext queryContext = null)
    {
      this.serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
      this.queryContext = queryContext;
    }
    
    IQbservable<TResult> IQbservableProvider.CreateQuery<TResult>(Expression expression)
    {
      return new KQueryStreamSet<TResult>(serviceScopeFactory, queryContext, expression);
    }
  }
}