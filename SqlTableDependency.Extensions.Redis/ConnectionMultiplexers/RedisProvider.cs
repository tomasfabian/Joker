using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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

    protected ISubscriber Subject { get; set; }

    #endregion

    #region Port

    public int Port { get; protected set; } = 6379;

    #endregion

    #region IsConnected

    public bool IsConnected
    {
      get => isConnected;
      set
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

    protected async Task<ISubscriber> CreateSubject(string url)
    {
      var configuration = CreateConfigurationOptions(url);

      connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configuration);

      IsConnected = connectionMultiplexer.IsConnected;

      Initialize();
      
      Subject = connectionMultiplexer.GetSubscriber();

      return Subject;
    }

    #endregion

    #region Initialize

    public void Initialize()
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

      whenIsConnectedChangesSubject.OnCompleted();

      connectionMultiplexer.ConnectionRestored -= OnConnectionFailed;
      connectionMultiplexer.ConnectionFailed -= OnConnectionRestored;

      using (connectionMultiplexer)
      {
      }

      IsConnected = false;
    }

    #endregion

    #endregion
  }
}