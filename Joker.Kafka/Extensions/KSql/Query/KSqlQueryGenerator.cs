using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  public class KSqlQueryGenerator : ExpressionVisitor, IKSqlQueryGenerator
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

      kSqlVisitor.Append($" FROM {InterceptStreamName(streamName)}");

      bool isFirst = true;

      foreach (var methodCallExpression in whereClauses)
      {
        if (isFirst)
        {
          kSqlVisitor.AppendLine("");
          kSqlVisitor.Append("WHERE ");
          
          isFirst = false;
        }
        else
          kSqlVisitor.Append(" AND ");

        kSqlVisitor.Visit(methodCallExpression);
      }

      if(ShouldEmitChanges)
        kSqlVisitor.Append(" EMIT CHANGES");

      if(Limit.HasValue)
        kSqlVisitor.Append($" LIMIT {Limit}");

      kSqlVisitor.Append(";");

      return kSqlVisitor.BuildKSql();
    }

    protected virtual string InterceptStreamName(string value)
    {
      return value + "s";
    }

    protected int? Limit;

    public override Expression? Visit(Expression? expression)
    {
      if (expression == null)
        return null;

      switch (expression.NodeType)
      {
        case ExpressionType.Constant:
          VisitConstant((ConstantExpression)expression);
          break;
        case ExpressionType.Call:
          VisitMethodCall((MethodCallExpression)expression);
          break;
      }

      return expression;
    }
    
    private string streamName;

    protected override Expression VisitConstant(ConstantExpression constantExpression)
    {
      if (constantExpression == null) throw new ArgumentNullException(nameof(constantExpression));

      var type = constantExpression.Type;
      
      var kStreamSetType = type.TryFindKStreamSetAncestor();

      if (kStreamSetType?.Name == typeof(KStreamSet<>).Name)
        streamName = constantExpression.Type.BaseType?.GenericTypeArguments[0].Name;

      return constantExpression;
    }

    protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
    {
      var methodInfo = methodCallExpression.Method;

      if (methodInfo.DeclaringType == typeof(QbservableExtensions) &&
          methodInfo.Name == nameof(QbservableExtensions.Select)) {
        
        LambdaExpression lambda = (LambdaExpression)StripQuotes(methodCallExpression.Arguments[1]);

        if (select == null)
          select = lambda;
        
        VisitChained(methodCallExpression);
      }

      if (methodInfo.DeclaringType == typeof(QbservableExtensions) &&
          methodInfo.Name == nameof(QbservableExtensions.Where))
      {
        VisitChained(methodCallExpression);

        LambdaExpression lambda = (LambdaExpression)StripQuotes(methodCallExpression.Arguments[1]);
        whereClauses.Enqueue(lambda.Body);
      } 

      if (methodInfo.DeclaringType == typeof(QbservableExtensions) &&
          methodInfo.Name == nameof(QbservableExtensions.Take))
      {
        var arg = (ConstantExpression)methodCallExpression.Arguments[1];
        Limit = (int)arg.Value;

        VisitChained(methodCallExpression);
      } 

      return methodCallExpression;
    }

    protected void VisitChained(MethodCallExpression methodCallExpression)
    {        
      var firstPart = methodCallExpression.Arguments[0];

      if(firstPart.NodeType == ExpressionType.Call || firstPart.NodeType == ExpressionType.Constant)
        Visit(firstPart);
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