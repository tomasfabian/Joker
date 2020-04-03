using System;

namespace Joker.Notifications
{
  public class VersionedTableDependencyStatus
  {  
    public enum TableDependencyStatuses
    {
      None,
      Starting,
      Started,
      WaitingForNotification,
      StopDueToCancellation,
      StopDueToError,
    }

    public VersionedTableDependencyStatus(TableDependencyStatuses tableDependencyStatus, DateTimeOffset timestamp)
    {
      TableDependencyStatus = tableDependencyStatus;
      Timestamp = timestamp;
    }

    public TableDependencyStatuses TableDependencyStatus { get; }
    public DateTimeOffset Timestamp { get; }
  }
}