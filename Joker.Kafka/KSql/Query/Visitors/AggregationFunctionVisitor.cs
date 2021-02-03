using System.Linq.Expressions;
using System.Text;
using Kafka.DotNet.ksqlDB.KSql.Linq;

namespace Kafka.DotNet.ksqlDB.KSql.Query.Visitors
{
  internal class AggregationFunctionVisitor : KSqlVisitor
  {
    public AggregationFunctionVisitor(StringBuilder stringBuilder, bool useTableAlias)
      : base(stringBuilder, useTableAlias)
    {
    }

    protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
    {
      var methodInfo = methodCallExpression.Method;

      switch (methodInfo.Name)
      {
        case nameof(IAggregations<object>.Sum):
        case nameof(IAggregations<object>.Avg):
        case nameof(IAggregations<object>.Min):
        case nameof(IAggregations<object>.Max):
        case nameof(IAggregations<object>.Sqrt):
        case nameof(IAggregations<object>.Sign):
          if (methodCallExpression.Arguments.Count == 1)
          {
            Append($"{methodInfo.Name.ToUpper()}(");
            Visit(methodCallExpression.Arguments[0]);
            Append(")");
          }

          break;
        case nameof(IAggregations.Count):
          if (methodCallExpression.Arguments.Count == 0)
          {
            Append("COUNT(*)");
          }

          break;
      }

      return methodCallExpression;
    }
  }
}