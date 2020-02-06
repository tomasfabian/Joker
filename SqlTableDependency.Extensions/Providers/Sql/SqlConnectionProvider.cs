using System;
using System.Data.SqlClient;
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
  }
}