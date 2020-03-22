using TableDependency.SqlClient.Base.Abstracts;
using TableDependency.SqlClient.Base.Enums;

namespace SqlTableDependency.Extensions
{
  public class SqlTableDependencySettings<TEntity> 
    where TEntity : class
  {
    public string SchemaName { get; set; }
    public IUpdateOfModel<TEntity> UpdateOf { get; set; }
    public ITableDependencyFilter Filter { get; set; }
    public DmlTriggerType NotifyOn { get; set; } = DmlTriggerType.All;
    public bool ExecuteUserPermissionCheck { get; set; } = true;
    public bool IncludeOldValues { get; set; }
  }
}