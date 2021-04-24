using System;
using Kafka.DotNet.ksqlDB.KSql.Linq.Statements;
using Kafka.DotNet.ksqlDB.KSql.Query;
using Kafka.DotNet.ksqlDB.KSql.Query.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Kafka.DotNet.ksqlDB.KSql.RestApi.Statements.Clauses
{
  internal sealed class WithOrAsClause : IWithOrAsClause
  {
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly QueryContext queryContext;

    public WithOrAsClause(IServiceScopeFactory serviceScopeFactory, QueryContext queryContext)
    {
      this.serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
      this.queryContext = queryContext ?? throw new ArgumentNullException(nameof(queryContext));
    }

    public IAsClause With(CreationMetadata creationMetadata)
    {
      string withClause = CreateStatements.GenerateWithClause(creationMetadata);

      queryContext.PropertyBag["statement"] = @$"{queryContext.PropertyBag["statement"]}
{withClause}";

      return this;
    }

    public ICreateStatement<T> As<T>(string entityName = null)
    {
      if (entityName == String.Empty)
        entityName = null;

      queryContext.StreamName = entityName;

      return new CreateStatement<T>(serviceScopeFactory, queryContext);
    }
  }
}