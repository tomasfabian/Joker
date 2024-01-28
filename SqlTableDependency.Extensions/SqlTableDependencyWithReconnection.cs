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
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SqlTableDependency.Extensions.Enums;
using SqlTableDependency.Extensions.Providers.Sql;
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
  public class SqlTableDependencyWithReconnection<TEntity> : SqlTableDependencyEx<TEntity>, ISqlTableDependencyWithReconnection<TEntity>
    where TEntity : class, new()
  {
    #region Constructors

    public SqlTableDependencyWithReconnection(string connectionString, string tableName = null, string schemaName = null, 
      IModelToTableMapper<TEntity> mapper = null, IUpdateOfModel<TEntity> updateOf = null, ITableDependencyFilter filter = null, 
      DmlTriggerType notifyOn = DmlTriggerType.All, bool executeUserPermissionCheck = true, bool includeOldValues = false, 
      SqlTableDependencySettings<TEntity> settings = null)
      : base(connectionString, tableName, schemaName, mapper, updateOf, filter, notifyOn, executeUserPermissionCheck, includeOldValues)
    {
      Settings = settings;

      _dataBaseObjectsNamingConvention = GetBaseObjectsNamingConvention();
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

    #region Settings

    internal SqlTableDependencySettings<TEntity> Settings { get; set; }

    #endregion

    #region UniqueName

    public virtual string UniqueName
    {
      get
      {
        if (string.IsNullOrEmpty(Settings?.FarServiceUniqueName))
          return Environment.MachineName;

        return Settings.FarServiceUniqueName;
      }
    }

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

    #endregion
  }
}