using System;

namespace SqlTableDependency.Extensions.Providers.Sql
{
  public interface ISqlConnectionProvider
  {
    bool CheckConversationHandler(string connectionString, string conversationHandle);
    bool TestConnection(string connectionString, TimeSpan testConnectionTimeout);
  }
}