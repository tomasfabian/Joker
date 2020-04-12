using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TableDependency.SqlClient.Enumerations;

namespace SqlTableDependency.Extensions.Providers.Sql
{
  public class SqlConnectionProvider : ISqlConnectionProvider
  {
    #region GetConversationHandler

    public Guid GetConversationHandler(string connectionString, string farService)
    {
      Guid result;

      using (var sqlConnection = new SqlConnection(connectionString))
      {
        sqlConnection.Open();
        string command =        
          $"SELECT conversation_handle FROM sys.conversation_endpoints WITH (NOLOCK) WHERE far_service = N'{farService}_Receiver' AND state in (N'{ConversationEndpointState.CO.ToString()}', N'{ConversationEndpointState.SI.ToString()}', N'{ConversationEndpointState.SO.ToString()}');";

        var sqlCommand = new SqlCommand(command, sqlConnection);
        var scalar = sqlCommand.ExecuteScalar();
        result = (Guid?) scalar ?? Guid.Empty;
        sqlConnection.Close();
      }

      return result;
    }

    #endregion

    #region IsServiceBrokerEnabled

    public bool IsServiceBrokerEnabled(string connectionString)
    {
      var database = GetDatabaseName(connectionString);

      bool result;

      using (var sqlConnection = new SqlConnection(connectionString))
      {
        sqlConnection.Open();
        string command =        
          $"SELECT d.is_broker_enabled FROM sys.databases d WHERE name = '{database}';";

        var sqlCommand = new SqlCommand(command, sqlConnection);
        var scalar = sqlCommand.ExecuteScalar();
        result = (bool?) scalar ?? false;
        sqlConnection.Close();
      }

      return result;
    }

    #endregion

    #region CheckConversationHandler

    public bool CheckConversationHandler(string connectionString, string conversationHandle)
    {
      bool result;

      using (var sqlConnection = new SqlConnection(connectionString))
      {
        sqlConnection.Open();
        string command =
          $"SELECT COUNT(*) FROM sys.conversation_endpoints WITH (NOLOCK) WHERE conversation_handle = N'{conversationHandle}' AND state in (N'{ConversationEndpointState.CO.ToString()}', N'{ConversationEndpointState.SI.ToString()}', N'{ConversationEndpointState.SO.ToString()}');";

        var sqlCommand = new SqlCommand(command, sqlConnection);
        result = (int)sqlCommand.ExecuteScalar() > 0;
        sqlConnection.Close();
      }

      return result;
    }

    #endregion
    
    #region TestConnection

    public bool TestConnection(string connectionString, TimeSpan testConnectionTimeout)
    {
      var sqlConnectionStringBuilder = new SqlConnectionStringBuilder
      {
        ConnectionString = connectionString,
        ConnectTimeout = (int)testConnectionTimeout.TotalSeconds
      };

      using (var connection = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
      {
        try
        {
          connection.Open();

          return true;
        }
        catch(Exception e)
        {
          return false;
        }
      }
    }

    #endregion

    #region ExecuteNonQueryAsync

    public static async Task<int> ExecuteNonQueryAsync(string connectionString, string command)
    {
      using (var sqlConnection = new SqlConnection(connectionString))
      {
        sqlConnection.Open();

        var sqlCommand = new SqlCommand(command, sqlConnection);
        var result = await sqlCommand.ExecuteNonQueryAsync();
        sqlConnection.Close();

        return result;
      }
    }

    #endregion

    #region EnableServiceBroker

    public async Task<int> EnableServiceBroker(string connectionString)
    {
      var database = GetDatabaseName(connectionString);

      string command = $"ALTER DATABASE [{database}] SET ENABLE_BROKER WITH ROLLBACK IMMEDIATE;";

      return await ExecuteNonQueryAsync(connectionString, command);
    }

    #endregion

    #region DisableServiceBroker

    public async Task<int> DisableServiceBroker(string connectionString)
    {
      var database = GetDatabaseName(connectionString);

      string command = $"ALTER DATABASE [{database}] SET DISABLE_BROKER WITH ROLLBACK IMMEDIATE;";

      return await ExecuteNonQueryAsync(connectionString, command);
    }
    
    #endregion

    #region SetSingleUserMode

    internal static async Task<int> SetSingleUserMode(string connectionString)
    {
      var database = GetDatabaseName(connectionString);

      string command = $@"ALTER DATABASE {database} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";

      return await ExecuteNonQueryAsync(connectionString, command);
    }

    #endregion

    #region SetMultiUserMode

    internal static  async Task<int> SetMultiUserMode(string connectionString)
    {
      var database = GetDatabaseName(connectionString);

      string command = $@"ALTER DATABASE {database} SET MULTI_USER WITH ROLLBACK IMMEDIATE;";

      return await ExecuteNonQueryAsync(connectionString, command);
    }

    #endregion

    #region GetDatabaseName

    private static string GetDatabaseName(string connectionString)
    {
      var sqlConnectionStringBuilder = new SqlConnectionStringBuilder
      {
        ConnectionString = connectionString
      };

      var database = sqlConnectionStringBuilder.InitialCatalog;

      return database;
    }

    #endregion

    #region KillSessions

    internal static void KillSessions(string connectionString)
    {
      string queryString =
        @"SELECT conn.session_id
        FROM sys.dm_exec_sessions AS s
        JOIN sys.dm_exec_connections AS conn
        ON s.session_id = conn.session_id
        WHERE login_Name = 'sa' AND program_name = 'Core .Net SqlClient Data Provider';";
      
      var sessions = new List<int>();

      using (SqlConnection sqlConnection = new SqlConnection(connectionString))
      {
        SqlCommand sqlCommand =
          new SqlCommand(queryString, sqlConnection);
        sqlConnection.Open();

        SqlDataReader reader = sqlCommand.ExecuteReader();

        while (reader.Read())
        {
          var dataRecord = ((IDataRecord) reader);

          var sessionId = (int) dataRecord[0];
          sessions.Add(sessionId);
        }

        reader.Close();
      }

      using (SqlConnection sqlConnection = new SqlConnection(connectionString))
      {        sqlConnection.Open();
     
        foreach (var sessionId in sessions)
        {
          var command = $"KILL {sessionId};";

          var killSessionCommand = new SqlCommand(command, sqlConnection);
          try
          {
            var result = killSessionCommand.ExecuteNonQuery();
          }
          catch (Exception e)
          {
            Console.WriteLine(e);
          }
        }
      }
    }

    #endregion
  }
}