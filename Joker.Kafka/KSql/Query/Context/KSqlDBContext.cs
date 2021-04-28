﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kafka.DotNet.ksqlDB.KSql.Linq;
using Kafka.DotNet.ksqlDB.KSql.RestApi;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Enums;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Parameters;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Statements.Clauses;
using Microsoft.Extensions.DependencyInjection;
#if !NETSTANDARD
using Microsoft.Extensions.DependencyInjection.Extensions;
#endif

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
    
    internal readonly KSqlDBContextQueryDependenciesProvider KSqlDBQueryContext = new();
    
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

    #region CreateStatements

    public IWithOrAsClause CreateStreamStatement(string streamName)
    {
      return CreateStatement(streamName, CreationType.Create, KSqlEntityType.Stream);
    }

    public IWithOrAsClause CreateOrReplaceStreamStatement(string streamName)
    {
      return CreateStatement(streamName, CreationType.CreateOrReplace, KSqlEntityType.Stream);
    }

    public IWithOrAsClause CreateTableStatement(string tableName)
    {
      return CreateStatement(tableName, CreationType.Create, KSqlEntityType.Table);
    }

    public IWithOrAsClause CreateOrReplaceTableStatement(string tableName)
    {
      return CreateStatement(tableName, CreationType.CreateOrReplace, KSqlEntityType.Table);
    }

    private IWithOrAsClause CreateStatement(string tableName, CreationType creationType, KSqlEntityType entityType)
    {
      var serviceScopeFactory = KSqlDBQueryContext.Initialize(contextOptions);

      if (tableName == String.Empty)
        tableName = null;

      var statementContext = new StatementContext
      {
        EntityName = tableName, 
        CreationType = creationType, 
        KSqlEntityType = entityType
      };

      return new WithOrAsClause(serviceScopeFactory, statementContext);
    }

    #endregion

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