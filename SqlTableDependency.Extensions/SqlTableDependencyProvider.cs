using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using SqlTableDependency.Extensions.Disposables;
using SqlTableDependency.Extensions.Enums;
using SqlTableDependency.Extensions.Notifications;
using SqlTableDependency.Extensions.Providers.Sql;
using TableDependency.SqlClient.Base;
using TableDependency.SqlClient.Base.Abstracts;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;

namespace SqlTableDependency.Extensions
{
  public abstract class SqlTableDependencyProvider<TEntity> : DisposableObject, ISqlTableDependencyProvider<TEntity>
      where TEntity : class, new()
  {
    #region Fields

    private readonly string connectionString;
    private readonly IScheduler scheduler;
    private readonly LifetimeScope lifetimeScope;

    #endregion

    #region Constructors

    protected SqlTableDependencyProvider(
      ConnectionStringSettings connectionStringSettings, 
      IScheduler scheduler, 
      LifetimeScope lifetimeScope)
      : this(connectionStringSettings.ConnectionString, scheduler, lifetimeScope)
    {
    }

    protected SqlTableDependencyProvider(
      string connectionString,
      IScheduler scheduler,
      LifetimeScope lifetimeScope)
    {
      if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));

      this.connectionString = connectionString;
      this.scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
      this.lifetimeScope = lifetimeScope;
    }

    #endregion

    #region Properties

    #region SqlConnectionProvider

    protected virtual ISqlConnectionProvider SqlConnectionProvider { get; } = new SqlConnectionProvider();

    #endregion

    #region WhenEntityRecordChanges

    private readonly Subject<RecordChangedNotification<TEntity>> whenEntityRecordChangesSubject = new Subject<RecordChangedNotification<TEntity>>();

    public IObservable<RecordChangedNotification<TEntity>> WhenEntityRecordChanges => whenEntityRecordChangesSubject.AsObservable();

    #endregion

    #region WhenStatusChanges

    private readonly ISubject<TableDependencyStatus> whenStatusChanges = new ReplaySubject<TableDependencyStatus>(1);

    public IObservable<TableDependencyStatus> WhenStatusChanges => whenStatusChanges.AsObservable();

    #endregion

    #region TableName

    protected virtual string TableName => typeof(TEntity).Name;

    #endregion

    #region IsDatabaseAvailable

    private readonly TimeSpan testConnectionTimeout = TimeSpan.FromSeconds(2);

    protected virtual bool IsDatabaseAvailable
    {
      get
      {
        var connected = SqlConnectionProvider.TestConnection(connectionString, testConnectionTimeout);

        return connected;
      }
    }

    #endregion

    #region ReconnectionTimeSpan

    public virtual TimeSpan ReconnectionTimeSpan => TimeSpan.FromSeconds(5);

    #endregion

    #endregion

    #region Methods

    #region SubscribeToEntityChanges

    public void SubscribeToEntityChanges()
    {
      TrySubscribeToTableChanges();
    }

    #endregion

    #region TryReconnect

    private readonly SerialDisposable reconnectSubscription = new SerialDisposable();

    private void TryReconnect()
    {
      reconnectSubscription.Disposable = Observable.Timer(ReconnectionTimeSpan, ReconnectionTimeSpan, scheduler)
        .Where(c => IsDatabaseAvailable)
        .Take(1)
        .Subscribe(_ =>
        {
          OnBeforeServiceBrokerSubscription();

          TrySubscribeToTableChanges();
        }, error => TryReconnect());
    }

    #endregion

    #region OnBeforeServiceBrokerSubscription

    protected virtual void OnBeforeServiceBrokerSubscription()
    {
    }

    #endregion

    #region CreateSqlTableDependency

    protected virtual ITableDependency<TEntity> CreateSqlTableDependency(IModelToTableMapper<TEntity> modelToTableMapper)
    {
      switch (lifetimeScope)
      {
        case LifetimeScope.ConnectionScope:
          return new SqlTableDependencyWithReconnection<TEntity>(connectionString, TableName, mapper: modelToTableMapper);
        case LifetimeScope.ApplicationScope:
          return new SqlTableDependencyWitApplicationScope<TEntity>(connectionString, TableName, mapper: modelToTableMapper);
        case LifetimeScope.UniqueScope:
          return new SqlTableDependencyWithUniqueScope<TEntity>(connectionString, TableName, mapper: modelToTableMapper);
        default:
          return null;
      }
    }

    #endregion

    #region TrySubscribeToTableChanges

    private ITableDependency<TEntity> sqlTableDependency;

    private void TrySubscribeToTableChanges()
    {
      if (lifetimeScope != LifetimeScope.ConnectionScope && sqlTableDependency != null)
      {
        sqlTableDependency.Start();
        return;
      }

      TryStopLastConnection();

      var modelToTableMapper = OnInitializeMapper(new ModelToTableMapper<TEntity>());

      try
      {
        sqlTableDependency = CreateSqlTableDependency(modelToTableMapper);

        sqlTableDependency.OnChanged += SqlTableDependencyOnChanged;
        sqlTableDependency.OnError += SqlTableDependencyOnError;
        sqlTableDependency.OnStatusChanged += SqlTableDependencyOnStatusChanged;

        sqlTableDependency.Start();
        
        OnConnected();
      }
      catch (Exception error)
      {
        OnError(error);

        TryReconnect();
      }
    }

    #endregion

    #region OnConnected

    protected virtual void OnConnected()
    {
    }

    #endregion

    #region OnError

    private int TheConversationHandleIsNotFound = 8426;

    protected virtual void OnError(Exception error)
    {
      sqlTableDependency?.Stop();

      if (error is SqlException sqlException && sqlException.Number == TheConversationHandleIsNotFound)
        TryStopLastConnection();

      whenStatusChanges.OnNext(TableDependencyStatus.StopDueToError);
    }

    #endregion

    #region SqlTableDependencyOnChanged

    private void SqlTableDependencyOnChanged(object sender, RecordChangedEventArgs<TEntity> eventArgs)
    {
      var entity = eventArgs.Entity;

      switch (eventArgs.ChangeType)
      {
        case ChangeType.Insert:
          OnInserted(entity);
          break;
        case ChangeType.Update:
          OnUpdated(entity);
          break;
        case ChangeType.Delete:
          OnDeleted(entity);
          break;
      }

      var recordChangedNotification = new RecordChangedNotification<TEntity>()
                                      {
                                        Entity = eventArgs.Entity,
                                        ChangeType = eventArgs.ChangeType
                                      };

      whenEntityRecordChangesSubject.OnNext(recordChangedNotification);
    }

    #endregion

    #region SqlTableDependencyOnStatusChanged

    protected virtual void SqlTableDependencyOnStatusChanged(object sender, StatusChangedEventArgs e)
    {
      whenStatusChanges.OnNext(e.Status);
    }

    #endregion

    #region SqlTableDependencyOnError

    protected virtual void SqlTableDependencyOnError(object sender, ErrorEventArgs e)
    {
      OnError(e.Error);

      TryReconnect();
    }

    #endregion

    #region OnInitializeMapper

    protected virtual ModelToTableMapper<TEntity> OnInitializeMapper(ModelToTableMapper<TEntity> modelToTableMapper)
    {
      return modelToTableMapper;
    }

    #endregion

    #region OnInserted

    protected virtual void OnInserted(TEntity entity)
    {
    }

    #endregion

    #region OnUpdated

    protected virtual void OnUpdated(TEntity entity)
    {
    }

    #endregion

    #region OnDeleted

    protected virtual void OnDeleted(TEntity entity)
    {
    }

    #endregion

    #region TryStopLastConnection

    protected void TryStopLastConnection()
    {
      if (sqlTableDependency == null)
        return;

      sqlTableDependency.OnError -= SqlTableDependencyOnError;
      sqlTableDependency.OnStatusChanged -= SqlTableDependencyOnStatusChanged;
      sqlTableDependency.OnChanged -= SqlTableDependencyOnChanged;

      try
      {
        sqlTableDependency.Dispose();
        sqlTableDependency = null;
      }
      catch (Exception e)
      {
        OnError(e);
      }
    }

    #endregion

    #region OnDispose

    protected override void OnDispose()
    {
      base.OnDispose();

      using (reconnectSubscription)
      using (whenEntityRecordChangesSubject)
      {
      }

      TryStopLastConnection();
    }

    #endregion

    #endregion
  }
}