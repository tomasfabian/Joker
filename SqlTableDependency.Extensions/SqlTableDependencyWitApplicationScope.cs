using SqlTableDependency.Extensions.Enums;
using TableDependency.SqlClient.Base.Abstracts;
using TableDependency.SqlClient.Base.Enums;

namespace SqlTableDependency.Extensions
{
  public class SqlTableDependencyWitApplicationScope<TEntity> : SqlTableDependencyWithReconnection<TEntity>
    where TEntity : class, new()
  {
    #region Constructors

    public SqlTableDependencyWitApplicationScope(string connectionString, string tableName = null, string schemaName = null, IModelToTableMapper<TEntity> mapper = null, IUpdateOfModel<TEntity> updateOf = null, ITableDependencyFilter filter = null, DmlTriggerType notifyOn = DmlTriggerType.All, bool executeUserPermissionCheck = true, bool includeOldValues = false)
      : base(connectionString, tableName, schemaName, mapper, updateOf, filter, notifyOn, executeUserPermissionCheck,
        includeOldValues)
    {
    }

    #endregion

    #region LifetimeScope

    public override LifetimeScope LifetimeScope { get; } = LifetimeScope.ApplicationScope;

    #endregion
  }
}