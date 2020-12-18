#region License
// TableDependency, SqlTableDependency
// Copyright (c) 2019-2020 Tomas Fabian. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SqlTableDependency.Extensions.Enums;
using SqlTableDependency.Extensions.Providers.Sql;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Abstracts;
using TableDependency.SqlClient.Base.Delegates;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.Exceptions;
using TableDependency.SqlClient.Base.Messages;
using TableDependency.SqlClient.Base.Utilities;
using TableDependency.SqlClient.Exceptions;
using TableDependency.SqlClient.Extensions;
using TableDependency.SqlClient.Messages;

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

    #region LifetimeScope

    public virtual LifetimeScope LifetimeScope { get; } = LifetimeScope.ConnectionScope;

    #endregion

    #region UniqueName

    public virtual string UniqueName => Environment.MachineName;

    #endregion

    #region CreateTrigger

    //temp fix for:
    //https://github.com/christiandelbianco/monitor-table-change-with-sqltabledependency/issues/188
    private const string CreateTrigger = @"CREATE TRIGGER [tr_{0}_Sender] ON {1} 
WITH EXECUTE AS SELF
AFTER {13} AS 
BEGIN
    SET NOCOUNT ON;

    DECLARE @rowsToProcess INT
    DECLARE @currentRow INT
    DECLARE @records XML
    DECLARE @theMessageContainer NVARCHAR(MAX)
    DECLARE @dmlType NVARCHAR(10)
    DECLARE @modifiedRecordsTable TABLE ([RowNumber] INT IDENTITY(1, 1), {2})
    DECLARE @exceptTable TABLE ([RowNumber] INT, {17})
	DECLARE @deletedTable TABLE ([RowNumber] INT IDENTITY(1, 1), {18})
    DECLARE @insertedTable TABLE ([RowNumber] INT IDENTITY(1, 1), {18})
    {5}
  
    IF NOT EXISTS(SELECT 1 FROM INSERTED)
    BEGIN
        SET @dmlType = '{12}'
        INSERT INTO @modifiedRecordsTable SELECT {3} FROM DELETED {14}
    END
    ELSE
    BEGIN
        IF NOT EXISTS(SELECT * FROM DELETED)
        BEGIN
            SET @dmlType = '{10}'
            INSERT INTO @modifiedRecordsTable SELECT {3} FROM INSERTED {14}
        END
        ELSE
        BEGIN
            {4}
        END
    END

    SELECT @rowsToProcess = COUNT(1) FROM @modifiedRecordsTable    

    BEGIN TRY
        WHILE @rowsToProcess > 0
        BEGIN
            SELECT	{6}
            FROM	@modifiedRecordsTable
            WHERE	[RowNumber] = @rowsToProcess
                
            IF @dmlType = '{10}' 
            BEGIN
                {7}
            END
        
            IF @dmlType = '{11}'
            BEGIN
                {8}
            END

            IF @dmlType = '{12}'
            BEGIN
                {9}
            END

            SET @rowsToProcess = @rowsToProcess - 1
        END{15}
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000)
        DECLARE @ErrorSeverity INT
        DECLARE @ErrorState INT

        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState) {16}
    END CATCH
END";

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

      CreateOrReuseConversation(timeOut, watchDogTimeOut);

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

      WriteTraceMessage(TraceLevel.Info, "Waiting for receiving " + _tableName + "'s records change notifications.", (Exception)null);
    }

    #endregion

    #region CreateOrReuseConversation

    private void CreateOrReuseConversation(int timeOut, int watchDogTimeOut)
    {
      switch (LifetimeScope)
      {
        case LifetimeScope.ConnectionScope:
          _processableMessages = CreateDatabaseObjects(timeOut, watchDogTimeOut);
          break;
        case LifetimeScope.ApplicationScope:

          if (ConversationHandle == Guid.Empty ||
              !SqlConnectionProvider.CheckConversationHandler(_connectionString, ConversationHandle.ToString()))
          {
            if (ConversationHandle != Guid.Empty)
              DropDatabaseObjects();

            _processableMessages = CreateDatabaseObjects(timeOut, watchDogTimeOut);
          }

          break;
        case LifetimeScope.UniqueScope:
          ConversationHandle = SqlConnectionProvider.GetConversationHandler(_connectionString, GetBaseObjectsNamingConvention());

          if (ConversationHandle == Guid.Empty)
            _processableMessages = CreateDatabaseObjects(timeOut, watchDogTimeOut);
          else
            _processableMessages = GetProcessableMessages();

          break;
      }
    }

    #endregion

    #region GetProcessableMessages

    protected virtual IList<string> GetProcessableMessages()
    {
      var processableMessages = new List<string>();

      var userInterestedColumns = _userInterestedColumns as TableColumnInfo[] ?? _userInterestedColumns.ToArray();
      var tableColumns = userInterestedColumns as IList<TableColumnInfo>;

      // Messages
      var startMessageInsert = string.Format(StartMessageTemplate, _dataBaseObjectsNamingConvention, ChangeType.Insert);
      processableMessages.Add(startMessageInsert);

      var startMessageUpdate = string.Format(StartMessageTemplate, _dataBaseObjectsNamingConvention, ChangeType.Update);
      processableMessages.Add(startMessageUpdate);

      var startMessageDelete = string.Format(StartMessageTemplate, _dataBaseObjectsNamingConvention, ChangeType.Delete);
      processableMessages.Add(startMessageDelete);

      var interestedColumns = userInterestedColumns ?? tableColumns.ToArray();
      foreach (var userInterestedColumn in interestedColumns)
      {
        var message = $"{_dataBaseObjectsNamingConvention}/{userInterestedColumn.Name}";
        processableMessages.Add(message);

        if (this.IncludeOldValues)
        {
          message = $"{_dataBaseObjectsNamingConvention}/{userInterestedColumn.Name}/old";
          processableMessages.Add(message);
        }
      }

      var endMessage = string.Format(EndMessageTemplate, _dataBaseObjectsNamingConvention);
      processableMessages.Add(endMessage);

      return processableMessages;
    }

    #endregion

    #region WaitForNotifications

    protected override async Task WaitForNotifications(
      CancellationToken cancellationToken,
      Delegate[] onChangeSubscribedList,
      Delegate[] onErrorSubscribedList,
      Delegate[] onStatusChangedSubscribedList,
      int timeOut,
      int timeOutWatchDog)
    {
      if (LifetimeScope == LifetimeScope.ConnectionScope)
      {
        await base.WaitForNotifications(cancellationToken, onChangeSubscribedList, onErrorSubscribedList, onStatusChangedSubscribedList, timeOut, timeOutWatchDog);

        return;
      }

      this.WriteTraceMessage(TraceLevel.Verbose, "Get in WaitForNotifications.");

      var messagesBag = this.CreateMessagesBag(this.Encoding, _processableMessages);
      var messageNumber = _userInterestedColumns.Count() * (this.IncludeOldValues ? 2 : 1) + 2;

      var waitForSqlScript =
        $"BEGIN CONVERSATION TIMER ('{this.ConversationHandle.ToString().ToUpper()}') TIMEOUT = " + timeOutWatchDog + ";" +
        $"WAITFOR (RECEIVE TOP({messageNumber}) [message_type_name], [message_body] FROM [{_schemaName}].[{_dataBaseObjectsNamingConvention}_Receiver]), TIMEOUT {timeOut * 1000};";

      this.NotifyListenersAboutStatus(onStatusChangedSubscribedList, TableDependencyStatus.Started);

      try
      {
        using (var sqlConnection = new SqlConnection(_connectionString))
        {
          await sqlConnection.OpenAsync(cancellationToken);
          this.WriteTraceMessage(TraceLevel.Verbose, "Connection opened.");
          this.NotifyListenersAboutStatus(onStatusChangedSubscribedList, TableDependencyStatus.WaitingForNotification);

          while (true)
          {
            messagesBag.Reset();

            using (var sqlCommand = new SqlCommand(waitForSqlScript, sqlConnection))
            {
              sqlCommand.CommandTimeout = 0;
              this.WriteTraceMessage(TraceLevel.Verbose, "Executing WAITFOR command.");

              using (var sqlDataReader = await sqlCommand.ExecuteReaderAsync(cancellationToken).WithCancellation(cancellationToken))
              {
                while (sqlDataReader.Read())
                {
                  var message = new Message(sqlDataReader.GetSqlString(0).Value, sqlDataReader.IsDBNull(1) ? null : sqlDataReader.GetSqlBytes(1).Value);
                  if (message.MessageType == SqlMessageTypes.ErrorType) throw new QueueContainingErrorMessageException();
                  messagesBag.AddMessage(message);
                  this.WriteTraceMessage(TraceLevel.Verbose, $"Received message type = {message.MessageType}.");
                }
              }
            }

            if (messagesBag.Status == MessagesBagStatus.Collecting)
            {
              throw new MessageMisalignedException("Received a number of messages lower than expected.");
            }

            if (messagesBag.Status == MessagesBagStatus.Ready)
            {
              this.WriteTraceMessage(TraceLevel.Verbose, "Message ready to be notified.");
              this.NotifyListenersAboutChange(onChangeSubscribedList, messagesBag);
              this.WriteTraceMessage(TraceLevel.Verbose, "Message notified.");
            }
          }
        }
      }
      catch (OperationCanceledException)
      {
        this.NotifyListenersAboutStatus(onStatusChangedSubscribedList, TableDependencyStatus.StopDueToCancellation);
        this.WriteTraceMessage(TraceLevel.Info, "Operation canceled.");
      }
      catch (AggregateException aggregateException)
      {
        this.NotifyListenersAboutStatus(onStatusChangedSubscribedList, TableDependencyStatus.StopDueToError);
        if (cancellationToken.IsCancellationRequested == false) this.NotifyListenersAboutError(onErrorSubscribedList, aggregateException.InnerException);
        this.WriteTraceMessage(TraceLevel.Error, "Exception in WaitForNotifications.", aggregateException.InnerException);
      }
      catch (SqlException sqlException)
      {
        this.NotifyListenersAboutStatus(onStatusChangedSubscribedList, TableDependencyStatus.StopDueToError);
        if (cancellationToken.IsCancellationRequested == false) this.NotifyListenersAboutError(onErrorSubscribedList, sqlException);
        this.WriteTraceMessage(TraceLevel.Error, "Exception in WaitForNotifications.", sqlException);
      }
      catch (Exception exception)
      {
        this.NotifyListenersAboutStatus(onStatusChangedSubscribedList, TableDependencyStatus.StopDueToError);
        if (cancellationToken.IsCancellationRequested == false) this.NotifyListenersAboutError(onErrorSubscribedList, exception);
        this.WriteTraceMessage(TraceLevel.Error, "Exception in WaitForNotifications.", exception);
      }
    }

    #endregion

    #region GetBaseObjectsNamingConvention

    protected override string GetBaseObjectsNamingConvention()
    {
      if (LifetimeScope == LifetimeScope.ConnectionScope)
        return base.GetBaseObjectsNamingConvention();

      var name = $"{_schemaName}_{_tableName}";

      if (LifetimeScope == LifetimeScope.UniqueScope)
        return $"{name}_{UniqueName}";

      if (OriginalGuid == Guid.Empty)
        OriginalGuid = Guid.NewGuid();

      return $"{name}_{OriginalGuid}";
    }

    #endregion

    #region Dispose

    protected override void Dispose(bool disposing)
    {
      if (disposing && LifetimeScope != LifetimeScope.UniqueScope)
        DropDatabaseObjects();

      base.Dispose(disposing);
    }

    #endregion

    #region CreateSqlServerDatabaseObjects
    
    //hopefully temp fix for:
    //https://github.com/christiandelbianco/monitor-table-change-with-sqltabledependency/issues/188
    protected override IList<string> CreateSqlServerDatabaseObjects(IEnumerable<TableColumnInfo> userInterestedColumns, string columnsForUpdateOf, int watchDogTimeOut)
    {
      var processableMessages = new List<string>();
      var tableColumns = userInterestedColumns as IList<TableColumnInfo> ?? userInterestedColumns.ToList();

      var columnsForModifiedRecordsTable = this.PrepareColumnListForTableVariable(tableColumns, this.IncludeOldValues);
      var columnsForExceptTable = this.PrepareColumnListForTableVariable(tableColumns, false);
      var columnsForDeletedTable = this.PrepareColumnListForTableVariable(tableColumns, false);

      using (var sqlConnection = new SqlConnection(_connectionString))
      {
        sqlConnection.Open();

        using (var transaction = sqlConnection.BeginTransaction())
        {
          var sqlCommand = new SqlCommand { Connection = sqlConnection, Transaction = transaction };

          // Messages
          var startMessageInsert = string.Format(StartMessageTemplate, _dataBaseObjectsNamingConvention, ChangeType.Insert);
          sqlCommand.CommandText = $"CREATE MESSAGE TYPE [{startMessageInsert}] VALIDATION = NONE;";
          sqlCommand.ExecuteNonQuery();
          this.WriteTraceMessage(TraceLevel.Verbose, $"Message {startMessageInsert} created.");
          processableMessages.Add(startMessageInsert);

          var startMessageUpdate = string.Format(StartMessageTemplate, _dataBaseObjectsNamingConvention, ChangeType.Update);
          sqlCommand.CommandText = $"CREATE MESSAGE TYPE [{startMessageUpdate}] VALIDATION = NONE;";
          sqlCommand.ExecuteNonQuery();
          this.WriteTraceMessage(TraceLevel.Verbose, $"Message {startMessageUpdate} created.");
          processableMessages.Add(startMessageUpdate);

          var startMessageDelete = string.Format(StartMessageTemplate, _dataBaseObjectsNamingConvention, ChangeType.Delete);
          sqlCommand.CommandText = $"CREATE MESSAGE TYPE [{startMessageDelete}] VALIDATION = NONE;";
          sqlCommand.ExecuteNonQuery();
          this.WriteTraceMessage(TraceLevel.Verbose, $"Message {startMessageDelete} created.");
          processableMessages.Add(startMessageDelete);

          var interestedColumns = userInterestedColumns as TableColumnInfo[] ?? tableColumns.ToArray();
          foreach (var userInterestedColumn in interestedColumns)
          {
            var message = $"{_dataBaseObjectsNamingConvention}/{userInterestedColumn.Name}";
            sqlCommand.CommandText = $"CREATE MESSAGE TYPE [{message}] VALIDATION = NONE;";
            sqlCommand.ExecuteNonQuery();
            this.WriteTraceMessage(TraceLevel.Verbose, $"Message {message} created.");
            processableMessages.Add(message);

            if (this.IncludeOldValues)
            {
              message = $"{_dataBaseObjectsNamingConvention}/{userInterestedColumn.Name}/old";
              sqlCommand.CommandText = $"CREATE MESSAGE TYPE [{message}] VALIDATION = NONE;";
              sqlCommand.ExecuteNonQuery();
              this.WriteTraceMessage(TraceLevel.Verbose, $"Message {message} created.");
              processableMessages.Add(message);
            }
          }

          var endMessage = string.Format(EndMessageTemplate, _dataBaseObjectsNamingConvention);
          sqlCommand.CommandText = $"CREATE MESSAGE TYPE [{endMessage}] VALIDATION = NONE;";
          sqlCommand.ExecuteNonQuery();
          this.WriteTraceMessage(TraceLevel.Verbose, $"Message {endMessage} created.");
          processableMessages.Add(endMessage);

          // Contract
          var contractBody = string.Join("," + Environment.NewLine, processableMessages.Select(message => $"[{message}] SENT BY INITIATOR"));
          sqlCommand.CommandText = $"CREATE CONTRACT [{_dataBaseObjectsNamingConvention}] ({contractBody})";
          sqlCommand.ExecuteNonQuery();
          this.WriteTraceMessage(TraceLevel.Verbose, $"Contract {_dataBaseObjectsNamingConvention} created.");

          // Queues
          sqlCommand.CommandText = $"CREATE QUEUE [{_schemaName}].[{_dataBaseObjectsNamingConvention}_Receiver] WITH STATUS = ON, RETENTION = OFF, POISON_MESSAGE_HANDLING (STATUS = OFF);";
          sqlCommand.ExecuteNonQuery();
          this.WriteTraceMessage(TraceLevel.Verbose, $"Queue {_dataBaseObjectsNamingConvention}_Receiver created.");

          sqlCommand.CommandText = $"CREATE QUEUE [{_schemaName}].[{_dataBaseObjectsNamingConvention}_Sender] WITH STATUS = ON, RETENTION = OFF, POISON_MESSAGE_HANDLING (STATUS = OFF);";
          sqlCommand.ExecuteNonQuery();
          this.WriteTraceMessage(TraceLevel.Verbose, $"Queue {_dataBaseObjectsNamingConvention}_Sender created.");

          // Services
          sqlCommand.CommandText = string.IsNullOrWhiteSpace(this.ServiceAuthorization)
              ? $"CREATE SERVICE [{_dataBaseObjectsNamingConvention}_Sender] ON QUEUE [{_schemaName}].[{_dataBaseObjectsNamingConvention}_Sender];"
              : $"CREATE SERVICE [{_dataBaseObjectsNamingConvention}_Sender] AUTHORIZATION [{this.ServiceAuthorization}] ON QUEUE [{_schemaName}].[{_dataBaseObjectsNamingConvention}_Sender];";
          sqlCommand.ExecuteNonQuery();
          this.WriteTraceMessage(TraceLevel.Verbose, $"Service broker {_dataBaseObjectsNamingConvention}_Sender created.");

          sqlCommand.CommandText = string.IsNullOrWhiteSpace(this.ServiceAuthorization)
              ? $"CREATE SERVICE [{_dataBaseObjectsNamingConvention}_Receiver] ON QUEUE [{_schemaName}].[{_dataBaseObjectsNamingConvention}_Receiver] ([{_dataBaseObjectsNamingConvention}]);"
              : $"CREATE SERVICE [{_dataBaseObjectsNamingConvention}_Receiver] AUTHORIZATION [{this.ServiceAuthorization}] ON QUEUE [{_schemaName}].[{_dataBaseObjectsNamingConvention}_Receiver] ([{_dataBaseObjectsNamingConvention}]);";
          sqlCommand.ExecuteNonQuery();
          this.WriteTraceMessage(TraceLevel.Verbose, $"Service broker {_dataBaseObjectsNamingConvention}_Receiver created.");

          // Activation Store Procedure
          var dropMessages = string.Join(Environment.NewLine, processableMessages.Select((pm, index) =>
          {
            if (index > 0) return this.Spacer(8) + string.Format("IF EXISTS (SELECT * FROM sys.service_message_types WITH (NOLOCK) WHERE name = N'{0}') DROP MESSAGE TYPE [{0}];", pm);
            return string.Format("IF EXISTS (SELECT * FROM sys.service_message_types WITH (NOLOCK) WHERE name = N'{0}') DROP MESSAGE TYPE [{0}];", pm);
          }));

          var dropAllScript = this.PrepareScriptDropAll(dropMessages);
          sqlCommand.CommandText = this.PrepareScriptProcedureQueueActivation(dropAllScript);
          sqlCommand.ExecuteNonQuery();
          this.WriteTraceMessage(TraceLevel.Verbose, $"Procedure {_dataBaseObjectsNamingConvention} created.");

          // Begin conversation
          this.ConversationHandle = this.BeginConversation(sqlCommand);
          this.WriteTraceMessage(TraceLevel.Verbose, $"Conversation with handler {this.ConversationHandle} started.");

          // Trigger
          var declareVariableStatement = this.PrepareDeclareVariableStatement(interestedColumns);
          var selectForSetVariablesStatement = this.PrepareSelectForSetVariables(interestedColumns);
          var sendInsertConversationStatements = this.PrepareSendConversation(ChangeType.Insert, interestedColumns);
          var sendUpdatedConversationStatements = this.PrepareSendConversation(ChangeType.Update, interestedColumns);
          var sendDeletedConversationStatements = this.PrepareSendConversation(ChangeType.Delete, interestedColumns);

          sqlCommand.CommandText = string.Format(
              CreateTrigger,
              _dataBaseObjectsNamingConvention,
              $"[{_schemaName}].[{_tableName}]",
              columnsForModifiedRecordsTable,
              this.PrepareColumnListForSelectFromTableVariable(tableColumns),
              this.PrepareInsertIntoTableVariableForUpdateChange(interestedColumns, columnsForUpdateOf),
              declareVariableStatement,
              selectForSetVariablesStatement,
              sendInsertConversationStatements,
              sendUpdatedConversationStatements,
              sendDeletedConversationStatements,
              ChangeType.Insert,
              ChangeType.Update,
              ChangeType.Delete,
              string.Join(", ", this.GetDmlTriggerType(_dmlTriggerType)),
              this.CreateWhereCondition(),
              this.PrepareTriggerLogScript(),
              this.ActivateDatabaseLogging ? " WITH LOG" : string.Empty,
              columnsForExceptTable,
              columnsForDeletedTable,
              this.ConversationHandle);

          sqlCommand.ExecuteNonQuery();
          this.WriteTraceMessage(TraceLevel.Verbose, $"Trigger {_dataBaseObjectsNamingConvention} created.");

          // Associate Activation Store Procedure to sender queue
          sqlCommand.CommandText = $"ALTER QUEUE [{_schemaName}].[{_dataBaseObjectsNamingConvention}_Sender] WITH ACTIVATION (PROCEDURE_NAME = [{_schemaName}].[{_dataBaseObjectsNamingConvention}_QueueActivationSender], MAX_QUEUE_READERS = 1, EXECUTE AS {this.QueueExecuteAs.ToUpper()}, STATUS = ON);";
          sqlCommand.ExecuteNonQuery();

          // Run the watch-dog
          sqlCommand.CommandText = $"BEGIN CONVERSATION TIMER ('{this.ConversationHandle.ToString().ToUpper()}') TIMEOUT = " + watchDogTimeOut + ";";
          sqlCommand.ExecuteNonQuery();
          this.WriteTraceMessage(TraceLevel.Verbose, "Watch dog started.");

          // Persist all objects
          transaction.Commit();
        }

        _databaseObjectsCreated = true;

        this.WriteTraceMessage(TraceLevel.Info, $"All OK! Database objects created with naming {_dataBaseObjectsNamingConvention}.");
      }

      return processableMessages;
    }

    #endregion

    #endregion
  }
}