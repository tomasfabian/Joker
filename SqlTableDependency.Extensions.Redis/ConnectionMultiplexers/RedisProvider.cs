using System.Threading.Tasks;
using SqlTableDependency.Extensions.Disposables;
using StackExchange.Redis;

namespace SqlTableDependency.Extensions.Redis.ConnectionMultiplexers
{
  public abstract class RedisProvider : DisposableObject, IRedisProvider
  {
    #region Fields

    private ConnectionMultiplexer connectionMultiplexer;

    #endregion

    #region Properties

    #region Subject

    protected ISubscriber Subject { get; set; }

    #endregion

    #region Port

    public int Port { get; protected set; } = 6379;

    #endregion

    #region IsConnected

    public bool IsConnected { get; set; }

    #endregion

    #endregion

    #region Methods

    #region CreateSubject

    protected async Task<ISubscriber> CreateSubject(string url)
    {
      var configuration = CreateConfigurationOptions(url);

      connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configuration);

      OnConnected();

      Subject = connectionMultiplexer.GetSubscriber();

      return Subject;
    }

    #endregion

    #region OnConnected

    protected void OnConnected()
    {
      IsConnected = true;
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

      using (connectionMultiplexer)
      {
      }

      IsConnected = false;
    }

    #endregion

    #endregion
  }
}