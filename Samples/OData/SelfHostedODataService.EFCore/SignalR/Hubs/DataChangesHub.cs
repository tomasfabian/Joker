using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Sample.Domain.Models;
using SqlTableDependency.Extensions.Notifications;

namespace SelfHostedODataService.SignalR.Hubs
{
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public class DataChangesHub : Hub<IDataChangesHub>
  {    
    public async Task SendDataChange(RecordChangedNotification<Product> recordChangedNotification)
    {
      await Clients.All.ReceiveDataChange(recordChangedNotification);
    }
  }
}