using TableDependency.SqlClient.Base.Enums;
using TableDependencyStatuses  = Joker.Notifications.VersionedTableDependencyStatus.TableDependencyStatuses;

namespace Joker.Redis.SqlTableDependency.Extensions
{
  internal static class TableDependencyStatusExtensions
  {
    internal static TableDependencyStatuses Convert(this TableDependencyStatus tableDependencyStatus)
    {
      switch (tableDependencyStatus)
      {
        case TableDependencyStatus.None:
          return TableDependencyStatuses.None;
        case TableDependencyStatus.Started:
          return TableDependencyStatuses.Started;
        case TableDependencyStatus.Starting:
          return TableDependencyStatuses.Starting;
        case TableDependencyStatus.StopDueToCancellation:
          return TableDependencyStatuses.StopDueToCancellation;
        case TableDependencyStatus.StopDueToError:
          return TableDependencyStatuses.StopDueToError;
        case TableDependencyStatus.WaitingForNotification:
          return TableDependencyStatuses.WaitingForNotification;
        default:
          return TableDependencyStatuses.None;
      }
    }
  }
}