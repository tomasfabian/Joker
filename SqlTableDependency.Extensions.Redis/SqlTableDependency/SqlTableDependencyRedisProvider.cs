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

    private readonly ISqlTableDependencyProvider<TEntity> sqlTableDependencyProvider;
    private readonly IRedisPublisher redisPublisher;

    #endregion

    #region Constructors

    protected SqlTableDependencyRedisProvider(ISqlTableDependencyProvider<TEntity> sqlTableDependencyProvider, IRedisPublisher redisPublisher)
    {
      this.sqlTableDependencyProvider = sqlTableDependencyProvider ?? throw new ArgumentNullException(nameof(sqlTableDependencyProvider));
      this.redisPublisher = redisPublisher ?? throw new ArgumentNullException(nameof(redisPublisher));

    }

    #endregion

    #region ChannelName

    protected virtual string ChannelName => typeof(TEntity).Name;

    #endregion

    #region Methods

    #region StartPublishing

    private IDisposable entityChangesSubscription;

    public SqlTableDependencyRedisProvider<TEntity> StartPublishing()
    {
      entityChangesSubscription =
        sqlTableDependencyProvider.WhenEntityRecordChanges
          .Subscribe(OnSqlTableDependencyRecordChanged);

      return this;
    }

    #endregion

    #region OnSqlTableDependencyRecordChanged

    protected virtual void OnSqlTableDependencyRecordChanged(RecordChangedEventArgs<TEntity> eventArgs)
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