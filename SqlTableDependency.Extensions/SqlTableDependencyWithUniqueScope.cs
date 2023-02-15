using SqlTableDependency.Extensions.Enums;
using TableDependency.SqlClient.Base.Abstracts;
using TableDependency.SqlClient.Base.Enums;

namespace SqlTableDependency.Extensions
{
  public class SqlTableDependencyWithUniqueScope<TEntity> : SqlTableDependencyWithReconnection<TEntity>
    where TEntity : class, new()
  {
    #region Constructors

    public SqlTableDependencyWithUniqueScope(string connectionString, string tableName = null, string schemaName = null,
      IModelToTableMapper<TEntity> mapper = null, IUpdateOfModel<TEntity> updateOf = null, ITableDependencyFilter filter = null,
      DmlTriggerType notifyOn = DmlTriggerType.All, bool executeUserPermissionCheck = true, bool includeOldValues = false,
      SqlTableDependencySettings<TEntity> sqlTableDependency = null)
      : base(connectionString, tableName, schemaName, mapper, updateOf, filter, notifyOn, executeUserPermissionCheck, includeOldValues, sqlTableDependency)
    {
    }

    #endregion

    #region LifetimeScope

    public override LifetimeScope LifetimeScope { get; } = LifetimeScope.UniqueScope;

    #endregion
  }
}