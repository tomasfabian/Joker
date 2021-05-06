using System;
using Kafka.DotNet.ksqlDB.Infrastructure.Extensions;

namespace Kafka.DotNet.ksqlDB.KSql.RestApi.Statements
{
  internal class CreateEntity
  {
    internal string KSqlTypeTranslator(Type type) {
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
  }
}