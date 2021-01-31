using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Kafka.DotNet.ksqlDB.Infrastructure.Extensions;
using Kafka.DotNet.ksqlDB.KSql.Linq;
using Kafka.DotNet.ksqlDB.KSql.Query.Functions;
using Kafka.DotNet.ksqlDB.KSql.Query.Visitors;

namespace Kafka.DotNet.ksqlDB.KSql.Query
{
  internal class KSqlVisitor : ExpressionVisitor
  {
    private readonly StringBuilder stringBuilder;

    internal StringBuilder StringBuilder => stringBuilder;

    public KSqlVisitor()
    {
      stringBuilder = new();
    }

    internal KSqlVisitor(StringBuilder stringBuilder)
    {
      this.stringBuilder = stringBuilder;
    }

    public string BuildKSql()
    {
      var ksql = stringBuilder.ToString();

      stringBuilder.Clear();

      return ksql;
    }

    public string BuildKSql(Expression expression)
    {
      stringBuilder.Clear();

      Visit(expression);

      return stringBuilder.ToString();
    }

    public override Expression? Visit(Expression? expression)
    {
      if (expression == null)
        return null;

      //https://docs.ksqldb.io/en/latest/developer-guide/ksqldb-reference/quick-reference/
      switch (expression.NodeType)
      {
        case ExpressionType.Constant:
          VisitConstant((ConstantExpression)expression);
          break;
          
        //arithmetic
        case ExpressionType.Add:
        case ExpressionType.Subtract: 
        case ExpressionType.Divide: 
        case ExpressionType.Multiply: 
        case ExpressionType.Modulo: 
        //conditionals
        case ExpressionType.AndAlso:
        case ExpressionType.OrElse:
        case ExpressionType.NotEqual:
        case ExpressionType.Equal:
        case ExpressionType.GreaterThan:
        case ExpressionType.GreaterThanOrEqual:
        case ExpressionType.LessThan:
        case ExpressionType.LessThanOrEqual:
          VisitBinary((BinaryExpression)expression);
          break;

        case ExpressionType.Lambda:
          base.Visit(expression);
          break;

        case ExpressionType.New:
          VisitNew((NewExpression)expression);
          break;

        case ExpressionType.MemberAccess:
          VisitMember((MemberExpression)expression);
          break;

        case ExpressionType.Call:
          VisitMethodCall((MethodCallExpression)expression);
          break;

        case ExpressionType.Convert:
        case ExpressionType.ConvertChecked:
          VisitUnary((UnaryExpression)expression);
          break;
      }

      return expression;
    }

    protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
    {      
      var methodInfo = methodCallExpression.Method;

      if (methodCallExpression.Object == null
          && methodInfo.DeclaringType.Name == nameof(KSqlFunctionsExtensions))
        new KSqlFunctionVisitor(stringBuilder).Visit(methodCallExpression);

      if (methodCallExpression.Object != null
          && (methodInfo.DeclaringType.Name == typeof(IAggregations<>).Name || methodInfo.DeclaringType.Name == nameof(IAggregations)))
      {
        new AggregationFunctionVisitor(stringBuilder).Visit(methodCallExpression);
      }

      if (methodCallExpression.Type == typeof(string))
      {
        if (methodInfo.Name == nameof(string.ToUpper))
        {
          Append("UCASE(");
          Visit(methodCallExpression.Object);
          Append(")");
        }
        if (methodInfo.Name == nameof(string.ToLower))
        {
          Append("LCASE(");
          Visit(methodCallExpression.Object);
          Append(")");
        }
      }

      return methodCallExpression;
    }

    protected void PrintFunctionArguments(IEnumerable<Expression> expressions)
    {
      Append("(");

      bool isFirst = true;

      foreach (var expression in expressions)
      {
        if (isFirst)
          isFirst = false;
        else
          stringBuilder.Append(ColumnsSeparator);

        Visit(expression);
      }

      Append(")");
    }

    protected override Expression VisitConstant(ConstantExpression constantExpression)
    {
      if (constantExpression == null) throw new ArgumentNullException(nameof(constantExpression));

      var value = constantExpression.Value;

      if (value is not string && value is IEnumerable enumerable)
      {
        Append(enumerable);
      }
      else if (value is string)
      {
        stringBuilder.Append($"'{value}'");
      }
      else
      {
        var stringValue = value != null ? value.ToString() : "NULL";

        stringBuilder.Append(stringValue ?? "Unknown");
      }

      return constantExpression;
    }

    private const string OperatorAnd = "AND";

    protected override Expression VisitBinary(BinaryExpression binaryExpression)
    {
      if (binaryExpression == null) throw new ArgumentNullException(nameof(binaryExpression));

      Visit(binaryExpression.Left);

      //https://docs.ksqldb.io/en/latest/reference/sql/appendix/
      string @operator = binaryExpression.NodeType switch
      {
        //arithmetic
        ExpressionType.Add => "+",
        ExpressionType.Subtract => "-",
        ExpressionType.Divide => "/",
        ExpressionType.Multiply => "*",
        ExpressionType.Modulo => "%",
        //conditionals
        ExpressionType.AndAlso => OperatorAnd,
        ExpressionType.OrElse => "OR",
        ExpressionType.Equal => "=",
        ExpressionType.NotEqual => "!=",
        ExpressionType.LessThan => "<",
        ExpressionType.LessThanOrEqual => "<=",
        ExpressionType.GreaterThan => ">",
        ExpressionType.GreaterThanOrEqual => ">=",
      };

      @operator = $" {@operator} ";

      Append(@operator);

      Visit(binaryExpression.Right);

      return binaryExpression;
    }

    protected override Expression VisitNew(NewExpression newExpression)
    {
      if (newExpression == null) throw new ArgumentNullException(nameof(newExpression));

      if (newExpression.Type.IsAnonymousType())
      {
        bool isFirst = true;

        foreach (var memberWithArguments in newExpression.Members.Zip(newExpression.Arguments))
        {
          if (isFirst)
            isFirst = false;
          else
            Append(ColumnsSeparator);

          if (memberWithArguments.Second.NodeType == ExpressionType.Call)
          {
            VisitMethodCall(memberWithArguments.Second as MethodCallExpression);
            Append(" ");
          }
          else if(memberWithArguments.Second is BinaryExpression)
          {
            Visit(memberWithArguments.Second);
            Append(" AS ");
            Append(memberWithArguments.First.Name);

            return newExpression;
          }

          Append(memberWithArguments.First.Name);
        }
      }

      return newExpression;
    }

    protected override Expression VisitMember(MemberExpression memberExpression)
    {
      if (memberExpression == null) throw new ArgumentNullException(nameof(memberExpression));

      if (memberExpression.Expression == null)
        return memberExpression;
      
      var memberName = memberExpression.Member.Name;

      if (memberExpression.Expression.NodeType == ExpressionType.Parameter)
      {
        Append(memberName);
      }
      else if (memberExpression.Expression.NodeType == ExpressionType.MemberAccess)
      {
        if (memberName == nameof(string.Length)) //TODO: check type
        {
          Append("LEN(");
          Visit(memberExpression.Expression);
          Append(")");
        }
      }

      return memberExpression;
    }

    protected override Expression VisitUnary(UnaryExpression unaryExpression)
    {
      if (unaryExpression == null) throw new ArgumentNullException(nameof(unaryExpression));

      switch (unaryExpression.NodeType)
      {
        case ExpressionType.Convert:
        case ExpressionType.ConvertChecked:
          return base.Visit(unaryExpression.Operand);
        default:
          return base.VisitUnary(unaryExpression);
      }
    }
    protected static Expression StripQuotes(Expression expression)
    {
      while (expression.NodeType == ExpressionType.Quote)
      {
        expression = ((UnaryExpression)expression).Operand;
      }

      return expression;
    }

    public void Append(string value)
    {
      stringBuilder.Append(value);
    }

    public void AppendLine(string value)
    {
      stringBuilder.AppendLine(value);
    }

    private const string ColumnsSeparator = ", ";

    protected void Append(IEnumerable enumerable)
    {
      bool isFirst = true;

      foreach (var constant in enumerable)
      {
        if (isFirst)
          isFirst = false;
        else
          stringBuilder.Append(ColumnsSeparator);

        stringBuilder.Append(constant);
      }
    }
  }
}