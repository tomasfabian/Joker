using System;
using System.Collections.Generic;
using System.Linq;

namespace Joker.OData.Extensions.Types
{
  internal static class TypeExtensions
  {
    internal static Type GetCollectionGenericType(this Type type)
    {
      if (type.IsGenericType) 
      {
        var genericTypeDefinition = type.GetGenericTypeDefinition();

        if (genericTypeDefinition.GetInterfaces()
          .Any( t => t.IsGenericType && 
                     t.GetGenericTypeDefinition() == typeof(ICollection<>)))
        {
          return type.GetGenericArguments()[0];
        }
      }

      return null;
    }
  }
}