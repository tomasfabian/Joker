using System;
using Newtonsoft.Json;
using SqlTableDependency.Extensions.Disposables;
using SqlTableDependency.Extensions.Redis.ConnectionMultiplexers;
using TableDependency.SqlClient.Base.EventArgs;

namespace SqlTableDependency.Extensions.Redis.SqlTableDependency
{
  public abstract class SqlTableDependencyRedisProvider<TEntity> : DisposableObject
    where TEntity : class, new()
  {
    #region Fields

    private readonly ISqlTableDependencyProvider sqlTableDependencyProvider;
    private readonly IRedisPublisher redisPublisher;
    private readonly IDisposable entityChangesSubscription;

    #endregion

    #region Constructors

    protected SqlTableDependencyRedisProvider(ISqlTableDependencyProvider<TEntity> sqlTableDependencyProvider, IRedisPublisher redisPublisher)
    {
      this.sqlTableDependencyProvider = sqlTableDependencyProvider ?? throw new ArgumentNullException(nameof(sqlTableDependencyProvider));
      this.redisPublisher = redisPublisher ?? throw new ArgumentNullException(nameof(redisPublisher));

      entityChangesSubscription =
        sqlTableDependencyProvider.WhenEntityRecordChanges
        .Subscribe(OnSqlTableDependencyOnChanged);
    }

    #endregion

    #region ChannelName

    protected virtual string ChannelName => nameof(TEntity);

    #endregion

    #region Methods

    #region OnqlTableDependencyOnChangedj

    protected void OnSqlTableDependencyOnChanged(RecordChangedEventArgs<TEntity> eventArgs)
    {
      string json = Serialize(eventArgs);

      redisPublisher.Publish(ChannelName, json);
    }

    #endregion

    #region Serialize

    protected virtual string Serialize(object value)
    {
      string json = JsonConvert.SerializeObject(value);

      return json;
    }

    #endregion

    #region OnDispose

    protected override void OnDispose()
    {
      base.OnDispose();

      using (entityChangesSubscription)
      {
      }
    }

    #endregion

    #endregion
  }
}