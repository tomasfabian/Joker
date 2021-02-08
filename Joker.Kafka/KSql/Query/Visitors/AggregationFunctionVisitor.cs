using System;
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
        case nameof(IAggregations.Count):
        case nameof(IAggregations.LongCount):
          if (methodCallExpression.Arguments.Count == 0)
          {
            Append($"{nameof(IAggregations.Count).ToUpper()}(*)");
          }
          if (methodCallExpression.Arguments.Count == 1)
          {
            Append($"{nameof(IAggregations.Count).ToUpper()}(");
            Visit(methodCallExpression.Arguments[0]);
            Append(")");
          }
          break;
        case nameof(IAggregations<object>.Avg):
        case nameof(IAggregations<object>.Min):
        case nameof(IAggregations<object>.Max):
        case nameof(IAggregations<object>.Sum):
          if (methodCallExpression.Arguments.Count == 1)
          {
            Append($"{methodInfo.Name.ToUpper()}(");
            Visit(methodCallExpression.Arguments[0]);
            Append(")");
          }

          break;
        case nameof(IAggregations<object>.TopK):
        case nameof(IAggregations<object>.TopKDistinct):
          if (methodCallExpression.Arguments.Count == 2)
          {
            Append($"{methodInfo.Name.ToUpper()}");
            PrintFunctionArguments(methodCallExpression.Arguments);
          }

          break;
        case nameof(IAggregations<object>.EarliestByOffset):
        case nameof(IAggregations<object>.LatestByOffset):
        case nameof(IAggregations<object>.EarliestByOffsetAllowNulls):
        case nameof(IAggregations<object>.LatestByOffsetAllowNulls):
          if (methodCallExpression.Arguments.Count == 1)
          {
            var functionName = GetFunctionName(methodInfo.Name);
            var ignoreNulls = !methodInfo.Name.ToLower().EndsWith("Nulls".ToLower());
            Append($"{functionName}(");
            Visit(methodCallExpression.Arguments[0]);
            Append($", {ignoreNulls})");
          }

          break;
      }

      return methodCallExpression;
    }

    private string GetFunctionName(string methodName)
    {
      switch (methodName)
      {
        case nameof(IAggregations<object>.EarliestByOffset):
        case nameof(IAggregations<object>.EarliestByOffsetAllowNulls):
          return "EARLIEST_BY_OFFSET";
        case nameof(IAggregations<object>.LatestByOffset):
        case nameof(IAggregations<object>.LatestByOffsetAllowNulls):
          return "LATEST_BY_OFFSET";
        default:
          throw new NotSupportedException();
      }
    }
  }
}