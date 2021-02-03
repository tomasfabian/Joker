using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Kafka.DotNet.ksqlDB.KSql.Query.Visitors
{
  internal sealed class KSqlJoinSelectFieldsVisitor : KSqlVisitor
  {
    internal KSqlJoinSelectFieldsVisitor(StringBuilder stringBuilder)
      : base(stringBuilder, useTableAlias: true)
    {
    }

    protected override void ProcessVisitNewMember((MemberInfo memberInfo, Expression expresion) v)
    {
      if (v.expresion.NodeType == ExpressionType.MemberAccess)
      {
        Visit(v.expresion);
              
        Append(" " + v.memberInfo.Name);
      }
      else
      {
        base.ProcessVisitNewMember(v);
      }
    }

    protected override Expression VisitMember(MemberExpression memberExpression)
    {
      if (memberExpression == null) throw new ArgumentNullException(nameof(memberExpression));

      if (memberExpression.Expression.NodeType == ExpressionType.Parameter)
      {
        Append(((ParameterExpression)memberExpression.Expression).Name);
        Append(".");
        Append(memberExpression.Member.Name);
      }
      else
        base.VisitMember(memberExpression);

      return memberExpression;
    }
  }
}