using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Abstracts;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.Utilities;

namespace SqlTableDependency.Extensions
{
  public class SqlTableDependencyEx<TEntity> : SqlTableDependency<TEntity>, ISqlTableDependencyWithReconnection<TEntity>
    where TEntity : class, new()
  {
    #region Constructors

    public SqlTableDependencyEx(string connectionString, string tableName = null,
      string schemaName = null, IModelToTableMapper<TEntity> mapper = null, IUpdateOfModel<TEntity> updateOf = null,
      ITableDependencyFilter filter = null, DmlTriggerType notifyOn = DmlTriggerType.All,
      bool executeUserPermissionCheck = true, bool includeOldValues = false)
      : base(connectionString, tableName, schemaName, mapper, updateOf, filter, notifyOn, executeUserPermissionCheck,
        includeOldValues)
    {
    }

    #endregion

    #region CreateTrigger

    //fix for:
    //https://github.com/christiandelbianco/monitor-table-change-with-sqltabledependency/issues/188
    protected const string CreateTrigger = @"CREATE TRIGGER [tr_{0}_Sender] ON {1} 
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

    #region CreateSqlServerDatabaseObjects

    //hopefully fix for:
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
            columnsForDeletedTable);

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
  }
}