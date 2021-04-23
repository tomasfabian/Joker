using Kafka.DotNet.ksqlDB.KSql.RestApi.Statements.Clauses;

namespace Kafka.DotNet.ksqlDB.KSql.Query.Context
{
  public interface IKSqlDBStatementsContext
  {
    IWithOrAsClause CreateStreamStatement(string streamName);
    IWithOrAsClause CreateOrReplaceStreamStatement(string streamName);
    IWithOrAsClause CreateTableStatement(string tableName);
    IWithOrAsClause CreateOrReplaceTableStatement(string tableName);
  }
}