using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Abstracts;
using TableDependency.SqlClient.Base.Delegates;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.Exceptions;
using TableDependency.SqlClient.Base.Messages;
using TableDependency.SqlClient.Enumerations;
using TableDependency.SqlClient.Exceptions;
using TableDependency.SqlClient.Extensions;
using TableDependency.SqlClient.Messages;

namespace SqlTableDependency.Extensions
{
  public class SqlTableDependencyWithReconnection<T> : SqlTableDependency<T> where T : class, new()
  {
    #region Constructors

    public SqlTableDependencyWithReconnection(string connectionString, string tableName = null, string schemaName = null, IModelToTableMapper<T> mapper = null, IUpdateOfModel<T> updateOf = null, ITableDependencyFilter filter = null, DmlTriggerType notifyOn = DmlTriggerType.All, bool executeUserPermissionCheck = true, bool includeOldValues = false) 
      : base(connectionString, tableName, schemaName, mapper, updateOf, filter, notifyOn, executeUserPermissionCheck, includeOldValues)
    {
    }

    #endregion

    #region Properties

    private Guid OriginalGuid { get; set; }

    #endregion

    #region Events

    public override event ErrorEventHandler OnError;

    public override event ChangedEventHandler<T> OnChanged;

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

      this.NotifyListenersAboutStatus(onStatusChangedSubscribedList, TableDependencyStatus.Starting);

      _disposed = false;

      if (ConversationHandle == Guid.Empty || !CheckConversationHandler())
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

    #region CheckConversationHandler

    private bool CheckConversationHandler()
    {
      bool result;

      using (var sqlConnection = new SqlConnection(_connectionString))
      {
        sqlConnection.Open();
        string command =
          $"SELECT COUNT(*) FROM sys.conversation_endpoints WITH (NOLOCK) WHERE conversation_handle = N'{ConversationHandle.ToString()}' AND state in (N'{ConversationEndpointState.CO.ToString()}', N'{ConversationEndpointState.SI.ToString()}', N'{ConversationEndpointState.SO.ToString()}');";

        var sqlCommand = new SqlCommand(command, sqlConnection);
        result = (int)sqlCommand.ExecuteScalar() > 0;
        sqlConnection.Close();
      }

      return result;
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