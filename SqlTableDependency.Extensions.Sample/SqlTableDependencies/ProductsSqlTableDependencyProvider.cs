using System;
using System.Configuration;
using System.Reactive.Concurrency;
using Sample.Domain.Models;
using SqlTableDependency.Extensions.Sample.Logging;
using TableDependency.SqlClient.Base;
using TableDependency.SqlClient.Base.EventArgs;

namespace SqlTableDependency.Extensions.Sample.SqlTableDependencies
{
  internal class ProductsSqlTableDependencyProvider : SqlTableDependencyProvider<Product>
  {
    #region Fields

    private readonly ILogger logger;

    #endregion

    #region Constructors

    internal ProductsSqlTableDependencyProvider(ConnectionStringSettings connectionStringSettings, IScheduler scheduler, ILogger logger)
      : base(connectionStringSettings, scheduler)
    {
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    internal ProductsSqlTableDependencyProvider(string connectionString, IScheduler scheduler, ILogger logger)
      : base(connectionString, scheduler)
    {
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #endregion

    #region Properties

    #region TableName

    protected override string TableName => base.TableName+"s";

    #endregion

    #region IsDatabaseAvailable

    protected override bool IsDatabaseAvailable
    {
      get
      {
        Console.WriteLine("Checking database connection...");

        bool isDatabaseAvailable =  base.IsDatabaseAvailable;

        Console.WriteLine($"IsDatabaseAvailable: {isDatabaseAvailable}");

        Console.WriteLine(Environment.NewLine);

        return isDatabaseAvailable;
      }
    }

    #endregion

    #endregion

    #region Methods

    #region OnInitializeMapper

    protected override ModelToTableMapper<Product> OnInitializeMapper(ModelToTableMapper<Product> modelToTableMapper)
    {
      modelToTableMapper.AddMapping(c => c.Id, nameof(Product.Id));

      return modelToTableMapper;
    }

    #endregion

    #region OnInserted

    protected override void OnInserted(Product product)
    {
      base.OnInserted(product);

      logger.Trace("Entity was added");

      LogChangeInfo(product);
    }

    #endregion

    #region OnUpdated

    protected override void OnUpdated(Product product)
    {
      base.OnUpdated(product);

      logger.Trace("Entity was modified");

      LogChangeInfo(product);
    }

    #endregion

    #region OnDeleted

    protected override void OnDeleted(Product product)
    {
      base.OnDeleted(product);

      logger.Trace("Entity was deleted");

      LogChangeInfo(product);
    }

    #endregion

    #region SqlTableDependencyOnStatusChanged

    protected override void SqlTableDependencyOnStatusChanged(object sender, StatusChangedEventArgs e)
    {
      base.SqlTableDependencyOnStatusChanged(sender, e);
      
      Console.WriteLine(Environment.NewLine);

      Console.WriteLine($"Status changed {e.Status}");
    }

    #endregion

    #region OnConnected

    protected override void OnConnected()
    {
      base.OnConnected();

      Console.WriteLine(Environment.NewLine);

      Console.WriteLine("Subscription started");
    }

    #endregion

    #region OnBeforeServiceBrokerSubscription

    protected override void OnBeforeServiceBrokerSubscription()
    {
      base.OnBeforeServiceBrokerSubscription();

      Console.WriteLine(Environment.NewLine);

      Console.WriteLine("Trying to reconnect...");
    }

    #endregion

    #region OnError

    protected override void OnError(Exception exception)
    {
      base.OnError(exception);

      Console.WriteLine(Environment.NewLine);
      Console.ForegroundColor = ConsoleColor.DarkRed;

      Console.WriteLine("Error: ");
      Console.WriteLine(exception.Message);

      Console.ResetColor();
    }

    #endregion

    #region LogChangeInfo

    private void LogChangeInfo(Product product)
    {
      Console.WriteLine(Environment.NewLine);

      Console.WriteLine($"Id: {product.Id}");
      Console.WriteLine($"Name: {product.Name}");

      Console.WriteLine("#####");
      Console.WriteLine(Environment.NewLine);
    }

    #endregion

    #endregion
  }
}