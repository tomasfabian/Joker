using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Sample.Domain.Models;
using SelfHostedODataService.SignalR.Hubs;
using SqlTableDependency.Extensions.Notifications;
using StackExchange.Redis;

namespace SelfHostedODataService.HostedServices
{
  internal class ProductChangesHostedService : IHostedService
  {
    private readonly IHubContext<DataChangesHub, IDataChangesHub> hubContext;
    private readonly IConnectionMultiplexer redis;

    public ProductChangesHostedService(
      IHubContext<DataChangesHub, IDataChangesHub> hubContext,
      IConnectionMultiplexer redis)
    {
      this.hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
      this.redis = redis ?? throw new ArgumentNullException(nameof(redis));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {      
      await redis.GetSubscriber().SubscribeAsync($"{nameof(Product)}-Changes", async (channel, value) => 
      {
        var recordChange = JsonConvert.DeserializeObject<RecordChangedNotification<Product>>(value);

        await hubContext.Clients.All.ReceiveDataChange(recordChange, cancellationToken);
      });
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      return Task.CompletedTask;
    }
  }
}