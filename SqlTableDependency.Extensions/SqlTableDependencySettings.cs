using TableDependency.SqlClient.Base.Abstracts;
using TableDependency.SqlClient.Base.Enums;

namespace SqlTableDependency.Extensions
{
  /// <summary>
  /// Additional settings for <see cref="TableDependency.SqlClient.SqlTableDependency{T}" /> class.
  /// </summary>
  public class SqlTableDependencySettings<TEntity> 
    where TEntity : class
  {
    /// <value>
    /// Name of the database schema.
    /// </value>
    public string SchemaName { get; set; }

    /// <value>
    /// Name of the table in database.
    /// </value>
    public string TableName { get; set; }

    /// <value>
    /// Far service name in the database. Applies only to <see cref="Enums.LifetimeScope.UniqueScope" />.
    /// </value>
    public string FarServiceUniqueName { get; set; }
    
    /// <value>
    /// List of columns that need to monitor for changing on order to receive notifications.
    /// </value>
    public IUpdateOfModel<TEntity> UpdateOf { get; set; }
    
    /// <value>
    /// The filter condition translated in WHERE.
    /// </value>
    public ITableDependencyFilter Filter { get; set; }
    
    /// <value>
    /// The notify on Insert, Delete, Update operation.
    /// </value>
    public DmlTriggerType NotifyOn { get; set; } = DmlTriggerType.All;
    
    /// <value>
    /// if set to <c>true</c> [skip user permission check].
    /// </value>
    public bool ExecuteUserPermissionCheck { get; set; } = true;
    
    /// <value>
    /// if set to <c>true</c> [include old values].
    /// </value>
    public bool IncludeOldValues { get; set; }

    /// <value>
    /// The WAITFOR timeout in seconds.
    /// </value>
    public int TimeOut { get; set; } = 120;

    /// <value>
    /// The WATCHDOG timeout in seconds.
    /// </value>
    public int WatchDogTimeOut { get; set; } = 180;
  }
}