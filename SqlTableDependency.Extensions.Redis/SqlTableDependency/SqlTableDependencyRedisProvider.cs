using System;
using Newtonsoft.Json;
using SqlTableDependency.Extensions.Disposables;
using SqlTableDependency.Extensions.Redis.ConnectionMultiplexers;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;

namespace SqlTableDependency.Extensions.Redis.SqlTableDependency
{
  public abstract class SqlTableDependencyRedisProvider<TEntity> : DisposableObject, ISqlTableDependencyRedisProvider<TEntity>
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

    #region ChangesChannelName

    protected virtual string ChangesChannelName => typeof(TEntity).Name + "-Changes";

    #endregion

    #region StatusChannelName

    protected virtual string StatusChannelName => typeof(TEntity).Name + "-Status";

    #endregion

    #region Methods

    #region StartPublishing

    private IDisposable entityChangesSubscription;
    private IDisposable statusChangesSubscription;

    public SqlTableDependencyRedisProvider<TEntity> StartPublishing()
    {
      entityChangesSubscription =
        sqlTableDependencyProvider.WhenEntityRecordChanges
          .Subscribe(OnSqlTableDependencyRecordChanged);

      statusChangesSubscription =
        sqlTableDependencyProvider.WhenStatusChanges
          .Subscribe(OnSqlTableDependencyStatusChanged);

      return this;
    }

    #endregion

    #region OnSqlTableDependencyRecordChanged

    protected virtual void OnSqlTableDependencyRecordChanged(RecordChangedEventArgs<TEntity> eventArgs)
    {
      var recordChangedNotification = new RecordChangedNotification<TEntity>()
                                      {
                                        Entity = eventArgs.Entity,
                                        ChangeType = eventArgs.ChangeType
                                      };

      string json = Serialize(recordChangedNotification);

      redisPublisher.PublishAsync(ChangesChannelName, json);
    }

    #endregion

    #region OnSqlTableDependencyStatusChanged

    protected virtual void OnSqlTableDependencyStatusChanged(TableDependencyStatus status)
    {
      string json = Serialize(status);

      redisPublisher.PublishAsync(StatusChannelName, json);
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
      using (statusChangesSubscription)
      {
      }
    }

    #endregion

    #endregion
  }
}