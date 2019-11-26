using TableDependency.SqlClient.Base.Enums;

namespace SqlTableDependency.Extensions.Redis.SqlTableDependency
{
  public class RecordChangedNotification<TEntity>
  {
    public TEntity Entity { get; set; }

    public ChangeType ChangeType { get; set; }
  }
}