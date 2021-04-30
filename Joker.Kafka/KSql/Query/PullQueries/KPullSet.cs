using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Kafka.DotNet.ksqlDB.KSql.Linq.PullQueries;
using Kafka.DotNet.ksqlDB.KSql.Query.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Kafka.DotNet.ksqlDB.KSql.Query.PullQueries
{
  internal abstract class KPullSet : KSet, IPullable
  {
    public IPullQueryProvider Provider { get; internal set; }
    
    internal QueryContext QueryContext { get; set; }
  }

  internal sealed class KPullSet<TEntity> : KPullSet, IPullable<TEntity>
  {
    private readonly IServiceScopeFactory serviceScopeFactory;
    private IServiceScope serviceScope;

    internal KPullSet(IServiceScopeFactory serviceScopeFactory, QueryContext queryContext = null)
    {
      this.serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

      QueryContext = queryContext;

      Provider = new PullQueryProvider(serviceScopeFactory, queryContext);

      Expression = Expression.Constant(this);
    }

    internal KPullSet(IServiceScopeFactory serviceScopeFactory, Expression expression, QueryContext queryContext = null)
    {
      this.serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

      QueryContext = queryContext;

      Provider = new PullQueryProvider(serviceScopeFactory, queryContext);

      Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    public override Type ElementType => typeof(TEntity);

    public ValueTask<TEntity> GetAsync(CancellationToken cancellationToken = default)
    {
      serviceScope = serviceScopeFactory.CreateScope();
      var dependencies = serviceScope.ServiceProvider.GetRequiredService<IKStreamSetDependencies>();

      cancellationToken.Register(() => serviceScope?.Dispose());

      dependencies.KSqlQueryGenerator.ShouldEmitChanges = false;

      var ksqlQuery = dependencies.KSqlQueryGenerator.BuildKSql(Expression, QueryContext);

      var ksqlDbProvider = dependencies.KsqlDBProvider;

      var queryParameters = dependencies.QueryStreamParameters;
      queryParameters.Sql = ksqlQuery;
      
      serviceScope.Dispose();

      return ksqlDbProvider.Run<TEntity>(queryParameters, cancellationToken)
        .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }
  }
}