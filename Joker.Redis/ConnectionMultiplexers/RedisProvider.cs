using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Joker.Disposables;
using StackExchange.Redis;

namespace Joker.Redis.ConnectionMultiplexers
{
  public abstract class RedisProvider : DisposableObject, IRedisProvider
  {
    #region Fields

    private IConnectionMultiplexer connectionMultiplexer;
    private bool isConnected;

    #endregion

    #region Properties

    #region Subject

    protected ISubscriber Subject { get; private set; }

    #endregion

    #region Port

    public int Port { get; protected set; } = 6379;

    #endregion

    #region IsConnected

    public bool IsConnected
    {
      get => isConnected;
      private set
      {
        if(isConnected == value)
          return;

        isConnected = value;

        whenIsConnectedChangesSubject.OnNext(isConnected);
      }
    }

    #endregion

    #region WhenIsConnectedChanges

    private readonly ISubject<bool> whenIsConnectedChangesSubject = new ReplaySubject<bool>(1);

    public IObservable<bool> WhenIsConnectedChanges => whenIsConnectedChangesSubject.StartWith(false).DistinctUntilChanged().AsObservable();

    #endregion

    #endregion

    #region Methods

    #region CreateconnectionMultiplexer

    internal virtual async Task<IConnectionMultiplexer> CreateConnectionMultiplexer(string url)
    {
      var configuration = CreateConfigurationOptions(url);

      return await ConnectionMultiplexer.ConnectAsync(configuration);
    }

    #endregion

    #region CreateSubject

    private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);

    protected async Task<ISubscriber> CreateSubject(string url)
    {
      await semaphore.WaitAsync();    

      try 
      {
        if(Subject == null)
        {      
          connectionMultiplexer = await CreateConnectionMultiplexer(url);

          IsConnected = connectionMultiplexer.IsConnected;

          Initialize();
      
          Subject = connectionMultiplexer.GetSubscriber();
        }
      }
      finally
      {
        semaphore.Release();
      }

      return Subject;
    }

    #endregion

    #region Initialize

    private void Initialize()
    {
      connectionMultiplexer.ConnectionRestored += OnConnectionRestored;
      connectionMultiplexer.ConnectionFailed += OnConnectionFailed;
    }

    #endregion

    #region OnConnectionRestored

    private void OnConnectionRestored(object sender, ConnectionFailedEventArgs args)
    {
      IsConnected = ((ConnectionMultiplexer)sender).IsConnected;
    }

    #endregion

    #region OnConnectionFailed

    private void OnConnectionFailed(object sender, ConnectionFailedEventArgs args)
    {
      IsConnected = false;
    }

    #endregion

    #region CreateConfigurationOptions

    private ConfigurationOptions CreateConfigurationOptions(string url)
    {
      var configuration = new ConfigurationOptions
                          {
                            AbortOnConnectFail = false,
                            ConnectTimeout = 3000,
                            SyncTimeout = 5000,
                            KeepAlive = 180,
                            EndPoints =
                            {
                              {
                                url, Port
                              }
                            }
                          };

      OnCreateConfigurationOptions(configuration);

      return configuration;
    }

    #endregion

    #region OnCreateConfigurationOptions

    protected virtual void OnCreateConfigurationOptions(ConfigurationOptions configurationOptions)
    {
    }

    #endregion

    #region GetDatabase

    protected IDatabase GetDatabase()
    {
      return connectionMultiplexer?.GetDatabase();
    }

    #endregion

    #region OnDispose

    protected override void OnDispose()
    {
      base.OnDispose();
      
      IsConnected = false;

      whenIsConnectedChangesSubject.OnCompleted();

      connectionMultiplexer.ConnectionRestored -= OnConnectionRestored;
      connectionMultiplexer.ConnectionFailed -= OnConnectionFailed;

      using (connectionMultiplexer)
      {
      }
    }

    #endregion

    #endregion
  }
}