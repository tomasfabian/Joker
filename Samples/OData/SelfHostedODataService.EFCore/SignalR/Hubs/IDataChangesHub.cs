using System.Threading;
using System.Threading.Tasks;
using Sample.Domain.Models;
using SqlTableDependency.Extensions.Notifications;

namespace SelfHostedODataService.SignalR.Hubs
{
  public interface IDataChangesHub
  {
    Task ReceiveDataChange(RecordChangedNotification<Product> recordChangedNotification);
    Task ReceiveDataChange(RecordChangedNotification<Product> recordChangedNotification, CancellationToken cancellationToken);
  }
}