using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Sample.Domain.Models;
using SqlTableDependency.Extensions.Enums;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;

namespace SqlTableDependency.Extensions.Sample
{
  internal class ProductsSqlTableDependencyProvider2 : SqlTableDependencyProvider<Product>
  {
    internal ProductsSqlTableDependencyProvider2(string connectionString, IScheduler scheduler, LifetimeScope lifetimeScope = LifetimeScope.UniqueScope)
      : base(connectionString, scheduler, lifetimeScope)
    {
    }

    protected override string TableName => base.TableName+"s";

    protected override SqlTableDependencySettings<Product> OnCreateSettings()
    {
      var settings = base.OnCreateSettings();

      settings.IncludeOldValues = true;

      return settings;
    }

    protected override ModelToTableMapper<Product> OnInitializeMapper(ModelToTableMapper<Product> modelToTableMapper)
    {
      modelToTableMapper.AddMapping(c => c.Id, "Id");
      modelToTableMapper.AddMapping(c => c.Name, "Name");

      return modelToTableMapper;
    }

    protected override void OnError(Exception error)
    {
      Console.WriteLine($"SqlTableDependency error {error}");
    }

    protected override void SqlTableDependencyOnStatusChanged(object sender, StatusChangedEventArgs e)
    {
      Console.WriteLine($"SqlTableDependency Status {e.Status}");
    }

    protected override void OnUpdated(Product entity, Product oldValues)
    {      
      Console.WriteLine("Id: " + entity.Id);
      Console.WriteLine("Name: " + entity.Name);

      if (oldValues != null)
      {
        Console.WriteLine(Environment.NewLine);

        Console.WriteLine("Id (OLD): " + oldValues.Id);
        Console.WriteLine("Name (OLD): " + oldValues.Name);
      }
    }

    public static void Example(string connectionString)
    {
      using var sqlTableDependency = new ProductsSqlTableDependencyProvider2(connectionString, ThreadPoolScheduler.Instance);
      sqlTableDependency.SubscribeToEntityChanges();

      Console.ReadKey();
    }
  }

  internal class ProductsSqlTableDependencyReactiveProvider : SqlTableDependencyProvider<Product>
  {
    internal ProductsSqlTableDependencyReactiveProvider(string connectionString, IScheduler scheduler, LifetimeScope lifetimeScope = LifetimeScope.UniqueScope)
      : base(connectionString, scheduler, lifetimeScope)
    {
    }

    protected override string TableName => base.TableName+"s";

    protected override SqlTableDependencySettings<Product> OnCreateSettings()
    {
      var settings = base.OnCreateSettings();

      settings.IncludeOldValues = true;

      return settings;
    }

    protected override ModelToTableMapper<Product> OnInitializeMapper(ModelToTableMapper<Product> modelToTableMapper)
    {
      modelToTableMapper.AddMapping(c => c.Id, "Id");
      modelToTableMapper.AddMapping(c => c.Name, "Name");

      return modelToTableMapper;
    }

    protected override void OnError(Exception error)
    {
      Console.WriteLine($"SqlTableDependency error {error}");
    }

    public static void Example(string connectionString)
    {
      using var sqlTableDependency = new ProductsSqlTableDependencyProvider2(connectionString, ThreadPoolScheduler.Instance);
      IDisposable whenEntityRecordChangesSubscription =
        sqlTableDependency.WhenEntityRecordChanges
          .Where(c => c.ChangeType == ChangeType.Update)
          .Subscribe(c =>
          {
            var entity = c.Entity;
            Console.WriteLine("Id: " + entity.Id);
            Console.WriteLine("Name: " + entity.Name);

            var oldValues = c.EntityOldValues;

            if (oldValues != null)
            {
              Console.WriteLine(Environment.NewLine);

              Console.WriteLine("Id (OLD): " + oldValues.Id);
              Console.WriteLine("Name (OLD): " + oldValues.Name);
            }
          });

      IDisposable whenStatusChangesSubscription =
        sqlTableDependency.WhenStatusChanges
          .Subscribe(status =>
          {
            Console.WriteLine($"SqlTableDependency Status {status}");
          });

      sqlTableDependency.SubscribeToEntityChanges();

      Console.ReadKey();

      whenEntityRecordChangesSubscription.Dispose();
      whenStatusChangesSubscription.Dispose();
    }
  }

  public static class ConversionFromSqlTableDependency
  {
    public static void SqlTableDependencyExample(string connectionString)
    {
      var mapper = new ModelToTableMapper<Product>();
      mapper.AddMapping(c => c.Id, "Id");
      mapper.AddMapping(c => c.Name, "Name");

      using var sqlTableDependency = new SqlTableDependency<Product>(connectionString, "Products", mapper: mapper, includeOldValues: true);
      sqlTableDependency.OnChanged += OnChanged;
      sqlTableDependency.OnError += OnError;
      sqlTableDependency.OnStatusChanged += OnStatusChanged;
      sqlTableDependency.Start();

      Console.ReadKey();

      sqlTableDependency.OnChanged -= OnChanged;
      sqlTableDependency.OnError -= OnError;
      sqlTableDependency.OnStatusChanged -= OnStatusChanged;
    }

    private static void OnError(object sender, ErrorEventArgs e)
    {
      Console.WriteLine($"SqlTableDependency error {e.Error}");
    }

    private static void OnStatusChanged(object sender, StatusChangedEventArgs e)
    {
      Console.WriteLine($"SqlTableDependency Status {e.Status}");
    }

    private static void OnChanged(object sender, RecordChangedEventArgs<Product> e)
    {
      if (e.ChangeType != ChangeType.Update)
        return;
      
      Console.WriteLine("Id: " + e.Entity.Id);
      Console.WriteLine("Name: " + e.Entity.Name);

      if(e.EntityOldValues != null)
      {
        Console.WriteLine(Environment.NewLine);

        var changedEntity = e.EntityOldValues;
        Console.WriteLine("Id (OLD): " + changedEntity.Id);
        Console.WriteLine("Name (OLD): " + changedEntity.Name);
      }
    }
  }
}