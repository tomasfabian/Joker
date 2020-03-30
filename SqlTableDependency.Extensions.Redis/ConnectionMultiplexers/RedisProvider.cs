using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using SqlTableDependency.Extensions.Disposables;
using StackExchange.Redis;

namespace SqlTableDependency.Extensions.Redis.ConnectionMultiplexers
{
  public abstract class RedisProvider : DisposableObject, IRedisProvider
  {
    #region Fields

    private ConnectionMultiplexer connectionMultiplexer;
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
        isConnected = value;

        whenIsConnectedChangesSubject.OnNext(isConnected);
      }
    }

    #endregion

    #region WhenIsConnectedChanges

    private readonly ISubject<bool> whenIsConnectedChangesSubject = new ReplaySubject<bool>(1);

    public IObservable<bool> WhenIsConnectedChanges => whenIsConnectedChangesSubject.DistinctUntilChanged().AsObservable();

    #endregion

    #endregion

    #region Methods
    
    #region CreateSubject

    private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);

    protected async Task<ISubscriber> CreateSubject(string url)
    {
      await semaphore.WaitAsync();    

      try 
      {
        if(Subject == null)
        {      
          var configuration = CreateConfigurationOptions(url);

          connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configuration);

          IsConnected = connectionMultiplexer.IsConnected;

          Initialize();
      
          Subject = connectionMultiplexer.GetSubscriber();
          
          Console.WriteLine("Subject created");
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
      IsConnected = true;
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