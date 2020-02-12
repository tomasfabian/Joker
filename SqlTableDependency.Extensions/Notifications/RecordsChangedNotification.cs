using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SqlTableDependency.Extensions.Notifications
{
  public class RecordsChangedNotification<TEntity> : Collection<RecordChangedNotification<TEntity>>
  {
    public RecordsChangedNotification(IList<RecordChangedNotification<TEntity>> list)
      : base(list)
    {
    }
  }
}