using System;
using System.Linq.Expressions;
using Kafka.DotNet.ksqlDB.KSql.Query;
using Kafka.DotNet.ksqlDB.KSql.Query.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Kafka.DotNet.ksqlDB.KSql.Linq.Statements
{
  internal class CreateStatementProvider : ICreateStatementProvider
  {
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly QueryContext queryContext;

    public CreateStatementProvider(IServiceScopeFactory serviceScopeFactory, QueryContext queryContext = null)
    {
      this.serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
      this.queryContext = queryContext;
    }
    
    ICreateStatement<TResult> ICreateStatementProvider.CreateStatement<TResult>(Expression expression)
    {
      return new CreateStatement<TResult>(serviceScopeFactory, expression, queryContext);
    }
  }
}