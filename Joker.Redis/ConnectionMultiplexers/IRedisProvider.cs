using System;

namespace Joker.Redis.ConnectionMultiplexers
{
  public interface IRedisProvider
  {
    int Port { get; }
    bool IsConnected { get; }
    IObservable<bool> WhenIsConnectedChanges { get; }
  }
}