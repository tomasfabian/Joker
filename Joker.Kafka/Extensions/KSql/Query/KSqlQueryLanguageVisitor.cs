using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Joker.Kafka.Extensions.KSql.Query
{
  public class KSqlQueryLanguageVisitor : ExpressionVisitor
  {
    private KSqlVisitor kSqlVisitor = new();

    public bool ShouldEmitChanges { get; set; } = true;

    public string BuildKSql(Expression expression)
    {
      kSqlVisitor = new KSqlVisitor();
      whereClauses = new Queue<Expression>();

      Visit(expression);

      bool isFirst = true;

      foreach (var methodCallExpression in whereClauses)
      {
        if (isFirst)
        {
          kSqlVisitor.Append("WHERE ");
          
          isFirst = false;
        }
        else
          kSqlVisitor.Append(" AND ");

        kSqlVisitor.Visit(methodCallExpression);
      }

      return kSqlVisitor.BuildKSql();
    }

    public override Expression? Visit(Expression? expression)
    {
      if (expression == null)
        return null;

      switch (expression.NodeType)
      {
        case ExpressionType.Call:
          VisitMethodCall((MethodCallExpression)expression);
          break;
      }

      return expression;
    }

    protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression) {
      if (methodCallExpression.Method.DeclaringType == typeof(Queryable) &&
          methodCallExpression.Method.Name == nameof(Queryable.Where)) {
        
        var firstPart = methodCallExpression.Arguments[0];
        if(firstPart.NodeType == ExpressionType.Call)
          Visit(firstPart);
        
        LambdaExpression lambda = (LambdaExpression)StripQuotes(methodCallExpression.Arguments[1]);
        whereClauses.Enqueue(lambda.Body);
      } 

      return methodCallExpression;
    }

    private Queue<Expression> whereClauses;

    protected static Expression StripQuotes(Expression expression) {
      while (expression.NodeType == ExpressionType.Quote) {
        expression = ((UnaryExpression)expression).Operand;
      }

      return expression;
    }
  }
}