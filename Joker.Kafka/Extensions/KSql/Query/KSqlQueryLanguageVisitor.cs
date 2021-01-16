using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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
      
      kSqlVisitor.Append("SELECT ");

      if(@select != null)
        kSqlVisitor.Visit(@select.Body);
      else
        kSqlVisitor.Append("*");

      kSqlVisitor.Append(" FROM "); //TODO: KStream or KTable name

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

      // if(ShouldEmitChanges)
        // kSqlVisitor.Append(" EMIT CHANGES;");

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
          methodCallExpression.Method.Name == nameof(Queryable.Select)) {
        
        LambdaExpression lambda = (LambdaExpression)StripQuotes(methodCallExpression.Arguments[1]);

        if (select == null)
          select = lambda;
        
        var firstPart = methodCallExpression.Arguments[0];
        if(firstPart.NodeType == ExpressionType.Call)
        {
          Visit(firstPart);
        }
      }

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
    private LambdaExpression select;

    protected static Expression StripQuotes(Expression expression) {
      while (expression.NodeType == ExpressionType.Quote) {
        expression = ((UnaryExpression)expression).Operand;
      }

      return expression;
    }
  }
}