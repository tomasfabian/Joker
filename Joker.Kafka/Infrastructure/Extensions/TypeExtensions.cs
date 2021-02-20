﻿using System;
using System.Runtime.CompilerServices;
using Kafka.DotNet.ksqlDB.KSql.Query;

namespace Kafka.DotNet.ksqlDB.Infrastructure.Extensions
{
  internal static class TypeExtensions
  {
    internal static bool IsAnonymousType(this Type type)
      => type.Name.StartsWith("<>", StringComparison.Ordinal)
         && type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), inherit: false).Length > 0
         && type.Name.Contains("AnonymousType");

    internal static Type TryFindKStreamSetAncestor(this Type type)
    {
      Type baseType = type.BaseType;

      while (baseType != null)
      {
        if (baseType.Name == typeof(KStreamSet<>).Name)
        {
          return baseType;
        }

        baseType = baseType.BaseType;
      }

      return null;
    }
    
    internal static bool IsStruct(this Type source) 
    {
      return source.IsValueType && !source.IsEnum && !source.IsPrimitive;
    }
  }
}