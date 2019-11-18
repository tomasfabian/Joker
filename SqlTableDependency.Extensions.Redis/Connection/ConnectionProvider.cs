using StackExchange.Redis;

namespace SqlTableDependency.Extensions.Redis.Connection
{
  internal class ConnectionProvider
  {
    #region Properties

    #region PubSubPort

    public int PubSubPort { get; protected set; } = 6379;

    #endregion

    #endregion

    #region Methods

    #region CreateConnection

    private ConnectionMultiplexer CreateConnection(string pubSubAddress = null)
    {
      var configuration = new ConfigurationOptions
                          {
                            AbortOnConnectFail = false,
                            ConnectTimeout = 5000,
                            SyncTimeout = 5000,
                            KeepAlive = 30,
                            EndPoints =
                            {
                              {
                                pubSubAddress, PubSubPort
                              }
                            }
                          };

      var connectionMultiplexer = ConnectionMultiplexer.Connect(configuration);

      return connectionMultiplexer;
    }

    #endregion

    #endregion
  }
}