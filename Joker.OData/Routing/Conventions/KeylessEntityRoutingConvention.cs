using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.OData.Routing.Conventions;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Joker.OData.Routing.Conventions
{
  public class KeylessEntityRoutingConvention : EntityRoutingConvention, IODataControllerActionConvention
  {
    public new int Order => 299;

    public override bool AppliesToAction(ODataControllerActionContext context)
    {
      if (context == null)
      {
        throw new ArgumentNullException(nameof(context));
      }

      ActionModel action = context.Action;
      IEdmEntitySet entitySet = context.EntitySet;
      IEdmEntityType entityType = entitySet.EntityType();

      string actionName = action.ActionMethod.Name;

      // We care about the action in this pattern: {HttpMethod}{EntityTypeName}
      //(string httpMethod, string castTypeName) = Split(actionName);
      var method = typeof(EntityRoutingConvention).GetMethod("Split", BindingFlags.Static | BindingFlags.NonPublic);


      (string httpMethod, string castTypeName) = ((string, string)) method.Invoke(obj: null, parameters: new object[] {actionName});
      if (httpMethod == null)
      {
        return false;
      }

      IEdmStructuredType castType = null;
      if (castTypeName != null)
      {
        castType = FindTypeInInheritance(entityType, context.Model, castTypeName);
        // castType = entityType.FindTypeInInheritance(context.Model, castTypeName);
        if (castType == null)
        {
          return false;
        }
      }

      //AddSelector(entitySet, entityType, castType, context.Prefix, context.Model, action, httpMethod);

      var method2 = typeof(EntityRoutingConvention).GetMethod("AddSelector", BindingFlags.Static | BindingFlags.NonPublic);

      method2.Invoke(obj: null, parameters: new object[] {entitySet, entityType, castType, context.Prefix, context.Model, action, httpMethod});
      
      return true;
    }

    private static IEdmStructuredType FindTypeInInheritance(IEdmStructuredType structuralType, IEdmModel model, string typeName)
    {
      IEdmStructuredType baseType = structuralType;

      while (baseType != null)
      {
        if (GetName(baseType) == typeName)
        {
          return baseType;
        }

        baseType = baseType.BaseType;
      }

      var derivedType = model.FindAllDerivedTypes(structuralType).FirstOrDefault(c => GetName(c) == typeName);

      return derivedType;
    }
    
    private static string GetName(IEdmStructuredType edmStructuredType)
    {
      if (edmStructuredType is IEdmEntityType entityType)
      {
        return entityType.Name;
      }

      return ((IEdmComplexType)edmStructuredType).Name;
    }
  }
}