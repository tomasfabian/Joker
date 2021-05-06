using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Kafka.DotNet.ksqlDB.Infrastructure.Extensions;
using Kafka.DotNet.ksqlDB.KSql.Query.Context;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Enums;

namespace Kafka.DotNet.ksqlDB.KSql.RestApi.Statements
{
  internal class CreateEntity
  {
    internal string KSqlTypeTranslator(Type type)
    {
      var ksqlType = string.Empty;

      if (type == typeof(string))
        ksqlType = "VARCHAR";
      if (type == typeof(int))
        ksqlType = "INT";
      if (type == typeof(long))
        ksqlType = "BIGINT";
      if (type == typeof(double))
        ksqlType = "DOUBLE";
      if (type == typeof(bool))
        ksqlType = "BOOLEAN";

      if (type.IsArray)
      {
        var elementType = KSqlTypeTranslator(type.GetElementType());

        ksqlType = $"ARRAY<{elementType}>";
      }

      if (type.IsDictionary())
      {
        Type[] typeParameters = type.GetGenericArguments();

        var keyType = KSqlTypeTranslator(typeParameters[0]);
        var valueType = KSqlTypeTranslator(typeParameters[1]);

        ksqlType = $"MAP<{keyType}, {valueType}>";
      }

      return ksqlType;
    }

    private readonly StringBuilder stringBuilder = new();

    internal string Print<T>(StatementContext statementContext, EntityCreationMetadata metadata, bool? ifNotExists)
    {
      stringBuilder.Clear();

      PrintCreateOrReplace<T>(statementContext);

      if (ifNotExists.HasValue && ifNotExists.Value)
        stringBuilder.Append(" IF NOT EXISTS");

      stringBuilder.Append($"{statementContext.Statement} {statementContext.EntityName}");

      stringBuilder.Append(" (" + Environment.NewLine);

      PrintProperties<T>(statementContext);

      stringBuilder.Append(")");

      string with = CreateStatements.GenerateWithClause(metadata);

      stringBuilder.Append($"{with};");

      return stringBuilder.ToString();
    }

    private void PrintProperties<T>(StatementContext statementContext)
    {
      var ksqlProperties = new List<string>();

      foreach (var propertyInfo in Properties<T>())
      {
        var ksqlType = KSqlTypeTranslator(propertyInfo.PropertyType);

        string columnDefinition = $"\t{propertyInfo.Name} {ksqlType}";

        columnDefinition += TryAttachKey(statementContext.KSqlEntityType, propertyInfo);

        ksqlProperties.Add(columnDefinition);
      }

      stringBuilder.AppendLine(string.Join($",{Environment.NewLine}", ksqlProperties));
    }

    private void PrintCreateOrReplace<T>(StatementContext statementContext)
    {
      string creationTypeText = statementContext.CreationType switch
      {
        CreationType.Create => "CREATE",
        CreationType.CreateOrReplace => "CREATE OR REPLACE",
      };

      string entityTypeText = statementContext.KSqlEntityType switch
      {
        KSqlEntityType.Table => KSqlEntityType.Table.ToString().ToUpper(),
        KSqlEntityType.Stream => KSqlEntityType.Stream.ToString().ToUpper(),
      };

      statementContext.EntityName = typeof(T).Name; // TODO: Pluralize

      stringBuilder.Append($"{creationTypeText} {entityTypeText}");
    }

    private string TryAttachKey(KSqlEntityType entityType, PropertyInfo propertyInfo)
    {
      if (!propertyInfo.HasKey())
        return string.Empty;

      string key = entityType switch
      {
        KSqlEntityType.Stream => "KEY",
        KSqlEntityType.Table => "PRIMARY KEY",
        _ => throw new ArgumentOutOfRangeException(nameof(entityType), entityType, null)
      };

      return $" {key}";
    }

    private IEnumerable<PropertyInfo> Properties<T>()
    {
      var properties = typeof(T)
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Where(c => c.CanWrite);

      return properties;
    }
  }
}