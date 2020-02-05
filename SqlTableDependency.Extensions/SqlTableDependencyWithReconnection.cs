using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SqlTableDependency.Extensions.Providers.Sql;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Abstracts;
using TableDependency.SqlClient.Base.Delegates;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.Exceptions;

namespace SqlTableDependency.Extensions
{
  public class SqlTableDependencyWithReconnection<TEntity> : SqlTableDependency<TEntity>, ISqlTableDependencyWithReconnection<TEntity>
    where TEntity : class, new()
  {
    #region Constructors

    public SqlTableDependencyWithReconnection(string connectionString, string tableName = null, string schemaName = null, IModelToTableMapper<TEntity> mapper = null, IUpdateOfModel<TEntity> updateOf = null, ITableDependencyFilter filter = null, DmlTriggerType notifyOn = DmlTriggerType.All, bool executeUserPermissionCheck = true, bool includeOldValues = false) 
      : base(connectionString, tableName, schemaName, mapper, updateOf, filter, notifyOn, executeUserPermissionCheck, includeOldValues)
    {
    }

    #endregion

    #region Properties

    #region OriginalGuid

    private Guid OriginalGuid { get; set; }

    #endregion

    #region SqlConnectionProvider

    public virtual ISqlConnectionProvider SqlConnectionProvider { get; } = new SqlConnectionProvider();

    #endregion

    #endregion

    #region Events

    public override event ErrorEventHandler OnError;

    public override event ChangedEventHandler<TEntity> OnChanged;

    public override event StatusEventHandler OnStatusChanged;

    #endregion

    #region Methods

    #region Start

    /// <summary>
    /// Starts monitoring table's content changes.
    /// </summary>
    /// <param name="timeOut">The WAITFOR timeout in seconds.</param>
    /// <param name="watchDogTimeOut">The WATCHDOG timeout in seconds.</param>
    /// <returns></returns>
    public override void Start(int timeOut = 120, int watchDogTimeOut = 180)
    {
      if (timeOut < 60) throw new ArgumentException("timeOut must be greater or equal to 60 seconds");
      if (watchDogTimeOut < 60 || watchDogTimeOut < (timeOut + 60)) throw new WatchDogTimeOutException("watchDogTimeOut must be at least 60 seconds bigger then timeOut");
      if (_task != null) return;

      if (OnChanged == null) throw new NoSubscriberException();

      var onChangedSubscribedList = OnChanged?.GetInvocationList();
      var onErrorSubscribedList = OnError?.GetInvocationList();
      var onStatusChangedSubscribedList = OnStatusChanged?.GetInvocationList();

      NotifyListenersAboutStatus(onStatusChangedSubscribedList, TableDependencyStatus.Starting);

      _disposed = false;

      if (ConversationHandle == Guid.Empty || !SqlConnectionProvider.CheckConversationHandler(_connectionString, ConversationHandle.ToString()))
      {
        if(ConversationHandle != Guid.Empty)
          DropDatabaseObjects();

        _processableMessages = CreateDatabaseObjects(timeOut, watchDogTimeOut);
      }

      _cancellationTokenSource = new CancellationTokenSource();

      _task = Task.Factory.StartNew(() =>
                                      WaitForNotifications(
                                        _cancellationTokenSource.Token,
                                        onChangedSubscribedList,
                                        onErrorSubscribedList,
                                        onStatusChangedSubscribedList,
                                        timeOut,
                                        watchDogTimeOut),
        _cancellationTokenSource.Token);
    }
    
    #endregion

    #region GetBaseObjectsNamingConvention

    protected override string GetBaseObjectsNamingConvention()
    {
      if (OriginalGuid == Guid.Empty)
        OriginalGuid = Guid.NewGuid();

      var name = $"{_schemaName}_{_tableName}";

      return $"{name}_{OriginalGuid}";
    }

    #endregion

    #region Stop

    public override void Stop()
    {
      if (_task != null)
      {
        _cancellationTokenSource.Cancel(true);
        _task?.Wait();
      }

      _task = null;
      _disposed = true;
      
      this.WriteTraceMessage(TraceLevel.Info, "Stopped waiting for notification.");
    }

    #endregion

    #region Dispose

    protected override void Dispose(bool disposing)
    {
      DropDatabaseObjects();
      
      base.Dispose(disposing);
    }

    #endregion

    #endregion
  }
}