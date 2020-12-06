using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Joker.Disposables;
using Joker.Extensions.Disposables;
using Joker.Notifications;
using TableDependencyStatuses  = Joker.Notifications.VersionedTableDependencyStatus.TableDependencyStatuses;
using Joker.Redis.ConnectionMultiplexers;
using Joker.Redis.SqlTableDependency.Extensions;
using Newtonsoft.Json;
using SqlTableDependency.Extensions;
using SqlTableDependency.Extensions.Notifications;
using TableDependency.SqlClient.Base.Enums;

namespace Joker.Redis.SqlTableDependency
{
  public abstract class SqlTableDependencyRedisProvider<TEntity> : DisposableObject, ISqlTableDependencyRedisProvider<TEntity>
    where TEntity : class, new()
  {
    #region Fields

    private readonly ISqlTableDependencyProvider<TEntity> sqlTableDependencyProvider;
    private readonly IRedisPublisher redisPublisher;
    private readonly IScheduler scheduler;

    #endregion

    #region Constructors

    protected SqlTableDependencyRedisProvider(ISqlTableDependencyProvider<TEntity> sqlTableDependencyProvider, IRedisPublisher redisPublisher, IScheduler scheduler)
    {
      this.sqlTableDependencyProvider = sqlTableDependencyProvider ?? throw new ArgumentNullException(nameof(sqlTableDependencyProvider));
      this.redisPublisher = redisPublisher ?? throw new ArgumentNullException(nameof(redisPublisher));
      this.scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));

      redisPublisher.SetStringRetryCount = 3;
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
      redisPublisher.WhenIsConnectedChanges
        .Where(isConnected => isConnected)
        .Subscribe(async isConnected =>
        {
          await PublishStatus(statusVersion);
        })
        .DisposeWith(this);

      entityChangesSubscription =
        sqlTableDependencyProvider.WhenEntityRecordChanges
          .ObserveOn(scheduler)
          .Finally(OnSqlTableDependencyRecordChangedSubscriptionFinished)
          .Subscribe(CreateSqlTableDependencyRecordChangedObserver());

      statusChangesSubscription =
        sqlTableDependencyProvider.WhenStatusChanges
          .ObserveOn(scheduler)
          .Subscribe(CreateSqlTableDependencyStatusChangedObserver());

      return this;
    }

    #endregion

    #region CreateSqlTableDependencyRecordChangedObserver

    protected virtual IObserver<RecordChangedNotification<TEntity>> CreateSqlTableDependencyRecordChangedObserver()
    {
      return Observer.Create<RecordChangedNotification<TEntity>>(OnSqlTableDependencyRecordChanged);
    }

    #endregion
    
    #region CreateSqlTableDependencyStatusChangedObserver

    protected virtual IObserver<TableDependencyStatus> CreateSqlTableDependencyStatusChangedObserver()
    {
      return Observer.Create<TableDependencyStatus>(async(s) => await OnSqlTableDependencyStatusChanged(s));
    }

    #endregion

    #region OnSqlTableDependencyRecordChangedSubscriptionFinished

    protected virtual void OnSqlTableDependencyRecordChangedSubscriptionFinished()
    {
    }

    #endregion

    #region OnSqlTableDependencyRecordChanged

    protected void OnSqlTableDependencyRecordChanged(RecordChangedNotification<TEntity> recordChangedNotification)
    {
      string jsonNotification = Serialize(recordChangedNotification);

      redisPublisher.PublishAsync(ChangesChannelName, jsonNotification);
    }

    #endregion

    #region OnSqlTableDependencyStatusChanged

    private VersionedTableDependencyStatus statusVersion = new VersionedTableDependencyStatus(TableDependencyStatuses.None, DateTimeOffset.MinValue);

    protected async Task OnSqlTableDependencyStatusChanged(TableDependencyStatus status)
    {
      statusVersion = new VersionedTableDependencyStatus(status.Convert(), DateTimeOffset.Now);

      await PublishStatus(statusVersion);
    }

    #endregion

    #region PublishStatus

    private async Task PublishStatus(VersionedTableDependencyStatus versionedTableDependencyStatus)
    {
      string jsonStatusVersion = Serialize(versionedTableDependencyStatus);

      await redisPublisher.SetStringAsync(StatusChannelName, jsonStatusVersion);

      await redisPublisher.PublishAsync(StatusChannelName, jsonStatusVersion);
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