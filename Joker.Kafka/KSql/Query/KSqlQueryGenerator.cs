﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Kafka.DotNet.ksqlDB.Infrastructure.Extensions;
using Kafka.DotNet.ksqlDB.KSql.Linq;
using Kafka.DotNet.ksqlDB.KSql.Query.Context;
using Kafka.DotNet.ksqlDB.KSql.Query.Visitors;
using Kafka.DotNet.ksqlDB.KSql.Query.Windows;
using Pluralize.NET;

namespace Kafka.DotNet.ksqlDB.KSql.Query
{
  internal class KSqlQueryGenerator : ExpressionVisitor, IKSqlQueryGenerator
  {
    private readonly KSqlDBContextOptions options;
    private static readonly IPluralize EnglishPluralizationService = new Pluralizer();

    private KSqlVisitor kSqlVisitor = new();

    public bool ShouldEmitChanges { get; set; } = true;

    public KSqlQueryGenerator(KSqlDBContextOptions options)
    {
      this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public string BuildKSql(Expression expression, QueryContext queryContext)
    {
      kSqlVisitor = new KSqlVisitor();
      whereClauses = new Queue<Expression>();
      joinTables = new List<(MethodInfo, IEnumerable<Expression>)>();

      Visit(expression);
      
      string finalStreamName = queryContext.StreamName ?? InterceptStreamName(streamName);

      if (joinTables.Any())
      {
        var joinsVisitor = new KSqlJoinsVisitor(kSqlVisitor.StringBuilder, options, new QueryContext { StreamName = finalStreamName });

        foreach (var joinTable in joinTables)
        {
          joinsVisitor.VisitJoinTable(joinTable);
        }
      }
      else
      {
        kSqlVisitor.Append("SELECT ");

        if (@select != null)
          kSqlVisitor.Visit(@select.Body);
        else
          kSqlVisitor.Append("*");

        kSqlVisitor.Append($" FROM {finalStreamName}");
      }

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

      TryGenerateWindowAggregation();

      if (groupBy != null)
      {
        kSqlVisitor.Append(" GROUP BY ");
        kSqlVisitor.Visit(groupBy.Body);
      }

      if (having != null)
      {
        kSqlVisitor.Append(" HAVING ");
        kSqlVisitor.Visit(having.Body);
      }

      if (ShouldEmitChanges)
        kSqlVisitor.Append(" EMIT CHANGES");

      if (Limit.HasValue)
        kSqlVisitor.Append($" LIMIT {Limit}");

      kSqlVisitor.Append(";");

      return kSqlVisitor.BuildKSql();
    }

    private void TryGenerateWindowAggregation()
    {
      if (windowedBy == null)
        return;

      var windowType = windowedBy switch
      {
        HoppingWindows _ => "HOPPING",
        SessionWindow _ => "SESSION",
        _ => "TUMBLING"
      };

      string size = windowType == "SESSION" ? String.Empty : "SIZE ";

      kSqlVisitor.Append($" WINDOW {windowType} ({size}{windowedBy.Duration.Value} {windowedBy.Duration.TimeUnit}");

      if(windowedBy is HoppingWindows {AdvanceBy: { }} hoppingWindows)
        kSqlVisitor.Append($", ADVANCE BY {hoppingWindows.AdvanceBy.Value} {hoppingWindows.AdvanceBy.TimeUnit}");

      if(windowedBy is HoppingWindows {Retention: { }} hoppingWindows2)
        kSqlVisitor.Append($", RETENTION {hoppingWindows2.Retention.Value} {hoppingWindows2.Retention.TimeUnit}");
      
      if (windowedBy.GracePeriod != null)
        kSqlVisitor.Append($", GRACE PERIOD {windowedBy.GracePeriod.Value} {windowedBy.GracePeriod.TimeUnit}");

      kSqlVisitor.Append(")");
    }

    protected virtual string InterceptStreamName(string value)
    {
      if (options.ShouldPluralizeStreamName)
        return EnglishPluralizationService.Pluralize(value);

      return value;
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

      if(methodInfo.DeclaringType != typeof(QbservableExtensions))
        return methodCallExpression;

      if (methodInfo.Name == nameof(QbservableExtensions.Select))
      {
        LambdaExpression lambda = (LambdaExpression)StripQuotes(methodCallExpression.Arguments[1]);

        if (select == null)
          select = lambda;

        VisitChained(methodCallExpression);
      }

      if (methodInfo.Name == nameof(QbservableExtensions.Where))
      {
        VisitChained(methodCallExpression);

        LambdaExpression lambda = (LambdaExpression)StripQuotes(methodCallExpression.Arguments[1]);
        whereClauses.Enqueue(lambda.Body);
      }

      if (methodInfo.Name == nameof(QbservableExtensions.Take))
      {
        var arg = (ConstantExpression)methodCallExpression.Arguments[1];
        Limit = (int)arg.Value;

        VisitChained(methodCallExpression);
      }

      if (methodInfo.Name == nameof(QbservableExtensions.WindowedBy))
      {
        var arg = (ConstantExpression)StripQuotes(methodCallExpression.Arguments[1]);
        windowedBy = (TimeWindows)arg.Value;

        VisitChained(methodCallExpression);
      }

      if (methodInfo.Name == nameof(QbservableExtensions.GroupBy))
      {
        groupBy = (LambdaExpression)StripQuotes(methodCallExpression.Arguments[1]);

        VisitChained(methodCallExpression);
      }

      if (methodInfo.Name == nameof(QbservableExtensions.Having))
      {
        having = (LambdaExpression)StripQuotes(methodCallExpression.Arguments[1]);

        VisitChained(methodCallExpression);
      }

      switch (methodInfo.Name)
      {
        case nameof(QbservableExtensions.Join):
        case nameof(QbservableExtensions.LeftJoin):
        case nameof(QbservableExtensions.FullOuterJoin):
          var joinTable = methodCallExpression.Arguments.Skip(1);
        
          joinTables.Add((methodInfo, joinTable));

          VisitChained(methodCallExpression);
          break;
      }

      return methodCallExpression;
    }

    protected void VisitChained(MethodCallExpression methodCallExpression)
    {
      var firstPart = methodCallExpression.Arguments[0];

      if (firstPart.NodeType == ExpressionType.Call || firstPart.NodeType == ExpressionType.Constant)
        Visit(firstPart);
    }

    private Queue<Expression> whereClauses;
    private LambdaExpression select;
    private TimeWindows windowedBy;
    private LambdaExpression groupBy;
    private LambdaExpression having;
    private List<(MethodInfo, IEnumerable<Expression>)> joinTables;

    protected static Expression StripQuotes(Expression expression)
    {
      while (expression.NodeType == ExpressionType.Quote)
      {
        expression = ((UnaryExpression)expression).Operand;
      }

      return expression;
    }
  }
}