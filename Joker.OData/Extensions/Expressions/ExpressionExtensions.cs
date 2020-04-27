using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Joker.OData.Extensions.Expressions
{
  internal static class ExpressionExtensions
  {
    internal static Expression<Func<TEntity, bool>> CreatePredicate<TEntity>(params object[] keys)
    {
      var keyProperties = KeyProperties<TEntity>();

      var predicate = CreatePredicate<TEntity>(keyProperties, keys);

      return predicate;
    }

    internal static List<PropertyInfo> KeyProperties<TEntity>()
    {
      return TryGetKeyProperties(typeof(TEntity));
    }

    internal static List<PropertyInfo> TryGetKeyProperties(this Type type)
    {
      var key = type.GetCustomAttributes().OfType<Microsoft.OData.Client.KeyAttribute>()
        .FirstOrDefault();

      if (key == null)
        throw new KeyNotFoundException();

      var keyProperties = type.GetProperties()
        .Where(c => key.KeyNames.Contains(c.Name))
        .ToList();
      return keyProperties;
    }

    internal static Expression<Func<TEntity, bool>> CreatePredicate<TEntity>(IEnumerable<PropertyInfo> keyProperties, params object[] keys)
    {
      return (from keyInfo in keyProperties.Zip(keys, (keyProperty, keyValue) => new { keyProperty, keyValue })
              let instanceParameter = Expression.Parameter(typeof(TEntity), "target")
              let memberAccessExpression = Expression.Property(instanceParameter, keyInfo.keyProperty)
              let equalsExpression = Expression.Equal(memberAccessExpression, Expression.Constant(keyInfo.keyValue))
              select Expression.Lambda<Func<TEntity, bool>>(equalsExpression, instanceParameter)).Aggregate(default(Expression<Func<TEntity, bool>>), (current, lambda) =>
            {
              if (current == null) return lambda;

              return current.And(lambda);
            });
    }

    internal static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
      expr2 = expr2.ReplaceInputParameterFrom(expr1);

      return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, expr2.Body), expr1.Parameters);
    }

    internal static Expression<Func<T, bool>> ReplaceInputParameterFrom<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
      var visitor = new UpdateParameterVisitor(expr1.Parameters[0], expr2.Parameters[0]);
      var body = visitor.Visit(expr1.Body);

      return Expression.Lambda<Func<T, bool>>(body, expr2.Parameters[0]);
    }

    private class UpdateParameterVisitor : ExpressionVisitor
    {
      private readonly ParameterExpression oldParameter;
      private readonly ParameterExpression newParameter;

      public UpdateParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
      {
        this.oldParameter = oldParameter;
        this.newParameter = newParameter;
      }

      protected override Expression VisitParameter(ParameterExpression node)
      {
        return ReferenceEquals(node, oldParameter) ? newParameter : base.VisitParameter(node);
      }
    }
  }
}