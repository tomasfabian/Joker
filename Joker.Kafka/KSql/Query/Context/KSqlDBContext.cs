using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kafka.DotNet.ksqlDB.KSql.Linq;
using Kafka.DotNet.ksqlDB.KSql.RestApi;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Parameters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kafka.DotNet.ksqlDB.KSql.Query.Context
{
  public class KSqlDBContext : KSqlDBContextDependenciesProvider, IKSqlDBContext
  {
    private readonly KSqlDBContextOptions contextOptions;

    public KSqlDBContext(string ksqlDbUrl)
      : this(new KSqlDBContextOptions(ksqlDbUrl))
    {
    }

    public KSqlDBContext(KSqlDBContextOptions contextOptions)
    {
      this.contextOptions = contextOptions ?? throw new ArgumentNullException(nameof(contextOptions));
    }
    
    private readonly KSqlDBContextQueryDependenciesProvider KSqlDBQueryContext = new();
    
#if !NETSTANDARD

    protected override void OnConfigureServices(IServiceCollection serviceCollection, KSqlDBContextOptions contextOptions)
    {
      base.OnConfigureServices(serviceCollection, contextOptions);
          
      serviceCollection.TryAddScoped<IKSqlDbProvider, KSqlDbQueryStreamProvider>();
          
      serviceCollection.TryAddSingleton<IQueryParameters>(contextOptions.QueryStreamParameters);
    }

    public IAsyncEnumerable<TEntity> CreateQueryStream<TEntity>(QueryStreamParameters queryStreamParameters, CancellationToken cancellationToken = default)
    {
      var serviceScopeFactory = Initialize(contextOptions);

      var ksqlDBProvider = serviceScopeFactory.CreateScope().ServiceProvider.GetService<IKSqlDbProvider>();

      return ksqlDBProvider.Run<TEntity>(queryStreamParameters, cancellationToken);
    }

    public IQbservable<TEntity> CreateQueryStream<TEntity>(string streamName = null)
    {
      var serviceScopeFactory = Initialize(contextOptions);

      if (streamName == String.Empty)
        streamName = null;

      var queryStreamContext = new QueryContext
      {
        StreamName = streamName
      };

      return new KQueryStreamSet<TEntity>(serviceScopeFactory, queryStreamContext);
    }

#endif
    
    public IAsyncEnumerable<TEntity> CreateQuery<TEntity>(QueryParameters queryParameters, CancellationToken cancellationToken = default)
    {
      var serviceScopeFactory = KSqlDBQueryContext.Initialize(contextOptions);

      var ksqlDBProvider = serviceScopeFactory.CreateScope().ServiceProvider.GetService<IKSqlDbProvider>();

      return ksqlDBProvider.Run<TEntity>(queryParameters, cancellationToken);
    }

    public IQbservable<TEntity> CreateQuery<TEntity>(string streamName = null)
    {
      var serviceScopeFactory = KSqlDBQueryContext.Initialize(contextOptions);

      if (streamName == String.Empty)
        streamName = null;

      var queryStreamContext = new QueryContext
      {
        StreamName = streamName
      };

      return new KQueryStreamSet<TEntity>(serviceScopeFactory, queryStreamContext);
    }

    protected override async ValueTask OnDisposeAsync()
    {
#if !NETSTANDARD
      await base.OnDisposeAsync();
#endif
      if(KSqlDBQueryContext != null)
        await KSqlDBQueryContext.DisposeAsync();
    }
  }
}