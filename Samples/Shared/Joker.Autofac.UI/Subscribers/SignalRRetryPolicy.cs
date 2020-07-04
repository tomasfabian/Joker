using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Joker.BlazorApp.Sample.Subscribers
{
  public class SignalRRetryPolicy : IRetryPolicy
  {
    public TimeSpan? NextRetryDelay(RetryContext retryContext)
    {
      if (retryContext.ElapsedTime < TimeSpan.FromSeconds(60))
      {
        Console.WriteLine("reconnecting 10s");
        return TimeSpan.FromSeconds(10);
      }
      
      Console.WriteLine("reconnecting 1m");
      return TimeSpan.FromMinutes(1);
    }
  }
}