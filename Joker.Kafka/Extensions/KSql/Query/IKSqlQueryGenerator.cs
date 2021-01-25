using System.Linq.Expressions;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  public interface IKSqlQueryGenerator
  {
    bool ShouldEmitChanges { get; set; }

    string BuildKSql(Expression expression, QueryContext queryContext);
  }
}