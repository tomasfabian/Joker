using System;
using Joker.WPF.Sample.Providers.Scheduling;
using Sample.Domain.Models;
using SqlTableDependency.Extensions;
using SqlTableDependency.Extensions.Enums;
using TableDependency.SqlClient.Base;
using TableDependency.SqlClient.Base.EventArgs;

namespace Joker.WPF.Sample.SqlTableDependencies
{
  internal class ProductsSqlTableDependencyProvider : SqlTableDependencyProvider<Product>
  {
    #region Fields


    #endregion

    #region Constructors

    public ProductsSqlTableDependencyProvider(string connectionString, ISchedulerProvider schedulerProvider, LifetimeScope lifetimeScope)
      : base(connectionString, schedulerProvider.ThreadPool, lifetimeScope)
    {
    }

    #endregion

    #region Properties

    #region TableName

    protected override string TableName => base.TableName+"s";

    #endregion

    #region IsDatabaseAvailable

    private readonly TimeSpan testConnectionTimeout = TimeSpan.FromSeconds(2);

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

      LogChangeInfo(product);
    }

    #endregion

    #region OnUpdated

    protected override void OnUpdated(Product product, Product oldValues)
    {
      base.OnUpdated(product, oldValues);

      LogChangeInfo(product);
    }

    #endregion

    #region OnDeleted

    protected override void OnDeleted(Product product)
    {
      base.OnDeleted(product);

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
      Console.WriteLine(Environment.NewLine);
      Console.ForegroundColor = ConsoleColor.DarkRed;

      Console.WriteLine("Error:");
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