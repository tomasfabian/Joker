using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Kafka.DotNet.ksqlDB.KSql.Linq;
using Kafka.DotNet.ksqlDB.KSql.Query.Context;
using Pluralize.NET;

namespace Kafka.DotNet.ksqlDB.KSql.Query.Visitors
{
  internal class KSqlJoinsVisitor : KSqlVisitor
  {
    private readonly KSqlDBContextOptions contextOptions;
    private readonly QueryContext queryContext;

    public KSqlJoinsVisitor(StringBuilder stringBuilder, KSqlDBContextOptions contextOptions, QueryContext queryContext)
      : base(stringBuilder, useTableAlias: false)
    {
      this.contextOptions = contextOptions ?? throw new ArgumentNullException(nameof(contextOptions));
      this.queryContext = queryContext ?? throw new ArgumentNullException(nameof(queryContext));
    }

    private readonly HashSet<string> aliasHashSet = new();

    private string GenerateAlias(string name)
    {
      var streamAlias = name[0].ToString();

      int i = 0;

      var streamAliasAttempt = streamAlias;

      while (aliasHashSet.Contains(streamAliasAttempt))
      {
        streamAliasAttempt = $"{streamAlias}{++i}";
      }
      
      aliasHashSet.Add(streamAliasAttempt);

      return streamAliasAttempt;
    }

    internal void VisitJoinTable(Expression[] expressions)
    {
      expressions = expressions.Select(StripQuotes).ToArray();
      
      Visit(expressions[0]);
      var outerStreamAlias = GenerateAlias(queryContext.StreamName);

      var streamAlias = GenerateAlias(streamName);

      var lambdaExpression = expressions[3] as LambdaExpression;
      var rewrittenAliases = PredicateReWriter.Rewrite(lambdaExpression, outerStreamAlias, streamAlias);

      Append("SELECT ");

      new KSqlJoinSelectFieldsVisitor(StringBuilder).Visit(rewrittenAliases);

      AppendLine($" FROM {queryContext.StreamName} {outerStreamAlias}");

      AppendLine($"INNER JOIN {streamName} {streamAlias}");
      Append($"ON {outerStreamAlias}.");
      Visit(expressions[1]);
      Append($" = {streamAlias}.");
      Visit(expressions[2]);
      AppendLine(string.Empty);
    }
    
    private string streamName;
    
    private static readonly IPluralize EnglishPluralizationService = new Pluralizer();

    protected virtual string InterceptStreamName(string value)
    {
      if (contextOptions.ShouldPluralizeStreamName)
        return EnglishPluralizationService.Pluralize(value);

      return value;
    }

    protected override Expression VisitConstant(ConstantExpression constantExpression)
    {
      if (constantExpression == null) throw new ArgumentNullException(nameof(constantExpression));

      if (constantExpression.Value is ISource source)
      {
        streamName = constantExpression.Type.GenericTypeArguments[0].Name;

        streamName = source?.QueryContext?.StreamName ?? InterceptStreamName(streamName);
      }

      return constantExpression;
    }
  }
}