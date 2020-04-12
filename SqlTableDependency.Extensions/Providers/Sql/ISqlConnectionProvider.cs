using System;
using System.Threading.Tasks;

namespace SqlTableDependency.Extensions.Providers.Sql
{
  public interface ISqlConnectionProvider
  {
    Guid GetConversationHandler(string connectionString, string farService);
    bool CheckConversationHandler(string connectionString, string conversationHandle);
    bool TestConnection(string connectionString, TimeSpan testConnectionTimeout);
    Task<int> EnableServiceBroker(string connectionString);
    Task<int> DisableServiceBroker(string connectionString);
  }
}