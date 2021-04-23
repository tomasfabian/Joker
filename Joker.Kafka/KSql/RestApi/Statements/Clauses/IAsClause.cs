using Kafka.DotNet.ksqlDB.KSql.Linq;

namespace Kafka.DotNet.ksqlDB.KSql.RestApi.Statements.Clauses
{
  public interface IAsClause
  {
    IQbservable<T> As<T>(string entityName = null);
  }
}