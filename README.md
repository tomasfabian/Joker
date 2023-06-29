Data change notifications from SQL Server via [SqlTableDependency](https://github.com/christiandelbianco/monitor-table-change-with-sqltabledependency), [OData](https://docs.microsoft.com/en-us/odata/overview) and [Redis](https://github.com/StackExchange/StackExchange.Redis) to different [.NET](https://dotnet.microsoft.com/) clients ([WinUI3 - UWP and Win32 apps](https://microsoft.github.io/microsoft-ui-xaml/about.html#what-is-it), [WPF](https://github.com/dotnet/wpf), [Blazor Wasm](https://docs.microsoft.com/sk-sk/aspnet/core/blazor/?view=aspnetcore-5.0#blazor-webassembly), etc). Blazor Wasm notifications are redirected with [SignalR](https://dotnet.microsoft.com/apps/aspnet/signalr).

<img src="jokerinaction.gif" alt="Joker in action" width="1024"/>

Set docker-compose.csproj as startup project in Joker.sln

# Joker Model-View-ViewModel:
Reactive view models for data changes

* [Joker.MVVM wiki](https://github.com/tomasfabian/SqlTableDependency.Extensions/wiki/Joker.MVVM)

Install-Package Joker.MVVM

# Joker OData:
Plumbing code for OData web services. Support for batching and end points. Please help out the community by sharing your suggestions and code improvements:
* [Joker.OData wiki](https://github.com/tomasfabian/SqlTableDependency.Extensions/wiki/Joker.OData)

# Preview:
[Redis TableDependency status notifier](https://github.com/tomasfabian/SqlTableDependency.Extensions/wiki/Redis-TableDependency-status-notifier---preview)
SQL server data changes refresher via Redis with End to end reconnections

# SqlTableDependency.Extensions
The `SqlTableDependency.Extensions` .NET package is a library that provides convenient and efficient ways to monitor and receive real-time notifications for changes in SQL Server database tables. It is built on top of the [SqlTableDependency](https://github.com/christiandelbianco/monitor-table-change-with-sqltabledependency) library and extends its functionality.

The main purpose of `SqlTableDependency.Extensions` is to simplify the process of setting up and handling database table change notifications in .NET applications.

With this package, you can easily subscribe to table changes and receive notifications in your application whenever a row is inserted, updated, or deleted in a specified SQL Server table. 

## Install:
https://www.nuget.org/packages/SqlTableDependency.Extensions/

Install-Package SqlTableDependency.Extensions

or

dotnet add package SqlTableDependency.Extensions --version 3.0.0

## See:
Following package is based on christiandelbianco's `SqlTableDependency`:
https://github.com/christiandelbianco/monitor-table-change-with-sqltabledependency

`SqlTableDependency.Extension.SqlTableDependencyProvider` provides periodic reconnections in case of any error, like lost connection etc.

Currently there are 3 LifetimeScopes:

## ConnectionScope:
If the connection is lost, the database objects will be deleted either after a timeout period or during disposal. Upon each reconnection, the database objects are recreated.

## ApplicationScope:
In case that the connection is lost, database objects will be deleted only after timeout period. After reconnection the database objects are recreated in case that the conversation handle does not exist anymore. Otherwise the database objects are preserved and reused. If the application was closed the conversation will not continue after app restart. You shouldn't lost data changes within the timeout period. The messages will be delivered after the reconnection.

## UniqueScope:
In case that the connection is lost, database objects will be deleted only after timeout period. After reconnection the database objects are recreated only in case, that the conversation handle does not exist anymore. Otherwise the database objects are preserved and reused. If the application was closed and the conversation was not cleaned it will be reused after app restarts. You shouldn't lost data changes within the timeout period. The messages will be delivered after the reconnection.

[Wiki Samples](https://github.com/tomasfabian/SqlTableDependency.Extensions/wiki/Samples)

## Docker for external dependencies:
MS SQL Server 2017:
```
docker run --name sql -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=<YourNewStrong@Passw0rd>" -p 1401:1433 -d mcr.microsoft.com/mssql/server:2017-latest
```
Redis latest:
```
docker run --name redis-server -p 6379:6379 -d redis
```

## Examples Entity Framework migrations:
Package Manager Console (Default project => Examples\Samples.Data):
```
Update-Database -ConnectionString "Server=127.0.0.1,1401;User Id = SA;Password=<YourNewStrong@Passw0rd>;Initial Catalog = Test;" -ConnectionProviderName "System.Data.SqlClient" -ProjectName Sample.Data -verbose
```
## Basic usage:

Enable Service Broker in MS SQL SERVER

```SQL
ALTER DATABASE [DatabaseName]
    SET ENABLE_BROKER WITH ROLLBACK IMMEDIATE;
```
// C#
```C#
  public class Product
  {
      public int Id { get; set; }
      public string Name { get; set; }
  } 
  
  using SqlTableDependency.Extensions;
  using SqlTableDependency.Extensions.Enums;
  
  internal class ProductsSqlTableDependencyProvider : SqlTableDependencyProvider<Product>
  {
    private readonly ILogger logger;

    internal ProductsSqlTableDependencyProvider(ConnectionStringSettings connectionStringSettings, IScheduler scheduler, ILogger logger)
      : base(connectionStringSettings, scheduler, LifetimeScope.UniqueScope)
    {
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    internal ProductsSqlTableDependencyProvider(string connectionString, IScheduler scheduler, ILogger logger)
      : base(connectionString, scheduler, LifetimeScope.UniqueScope)
    {
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override ModelToTableMapper<Product> OnInitializeMapper(ModelToTableMapper<Product> modelToTableMapper)
    {
      modelToTableMapper.AddMapping(c => c.Id, nameof(Product.Id));

      return modelToTableMapper;
    }

    protected override void OnInserted(Product product)
    {
      base.OnInserted(product);

      logger.Trace("Entity was added");

      LogChangeInfo(product);
    }

    protected override void OnUpdated(Product product, Product oldValues)
    {
      base.OnUpdated(entity, oldValues);

      logger.Trace("Entity was modified");

      LogChangeInfo(product);
    }

    protected override void OnDeleted(Product product)
    {
      base.OnDeleted(product);

      logger.Trace("Entity was deleted");

      LogChangeInfo(product);
    }

    private void LogChangeInfo(Product product)
    {
      Console.WriteLine(Environment.NewLine);

      Console.WriteLine("Id: " + product.Id);
      Console.WriteLine("Name: " + product.Name);

      Console.WriteLine("#####");
      Console.WriteLine(Environment.NewLine);
    }
    
    protected override void OnError(Exception exception)
    {
      logSource.Error(exception);
    }
  }
```
//Program.cs
```C#
using System.Configuration;
using System.Reactive.Concurrency;

namespace SqlTableDependency.Extensions.Sample
{
  class Program
  {
    static void Main(string[] args)
    {
      var connectionString = ConfigurationManager.ConnectionStrings["FargoEntities"].ConnectionString;
      
      using var productsProvider = new ProductsSqlTableDependencyProvider(connectionString, ThreadPoolScheduler.Instance, new ConsoleLogger());
      productsProvider.SubscribeToEntityChanges();
      
      Console.ReadKey();
    }
  }
}

```

# Joker.Redis
SqlServer PubSub notifications via Redis and SqlTableDependencyProvider extension

Install-Package Joker.Redis

Download and run redis-server (https://redis.io/download) or use Docker (see above).

Server side:
```C#
public class ProductSqlTableDependencyRedisProvider : SqlTableDependencyRedisProvider<Product>
{
  public ProductSqlTableDependencyRedisProvider(ISqlTableDependencyProvider<Product> sqlTableDependencyProvider, IRedisPublisher redisPublisher) 
    : base(sqlTableDependencyProvider, redisPublisher)
  {
  }
}

```

```C#
string redisUrl = ConfigurationManager.AppSettings["RedisUrl"]; //localhost

var redisPublisher = new RedisPublisher(redisUrl);
await redisPublisher.PublishAsync("messages", "hello");

using var productsProvider = new ProductsSqlTableDependencyProvider(connectionString, ThreadPoolScheduler.Instance, new ConsoleLogger());

using var productSqlTableDependencyRedisProvider = new ProductSqlTableDependencyRedisProvider(productsProvider, redisPublisher, ThreadPoolScheduler.Instance)
  .StartPublishing();
```

Client side:
```C#
private static async Task<RedisSubscriber> CreateRedisSubscriber(string redisUrl)
{
  var redisSubscriber = new RedisSubscriber(redisUrl);

  await redisSubscriber.Subscribe(channelMessage => { Console.WriteLine($"OnNext({channelMessage.Message})"); },
  "messages");

  await redisSubscriber.Subscribe(channelMessage =>
  {
    var recordChange = JsonConvert.DeserializeObject<RecordChangedNotification<Product>>(channelMessage.Message);
    Console.WriteLine($"OnNext Product changed({recordChange.ChangeType})");
    Console.WriteLine($"OnNext Product changed({recordChange.Entity.Id})");
  }, nameof(Product) + "-Changes");

  await redisSubscriber.Subscribe(channelMessage =>
  {
    var tableDependencyStatus = JsonConvert.DeserializeObject<TableDependencyStatus>(channelMessage.Message);
    Console.WriteLine($"OnNext tableDependencyStatus changed({tableDependencyStatus})");
  }, nameof(Product) + "-Status");

  redisSubscriber.WhenIsConnectedChanges.Subscribe(c => Console.WriteLine($"REDIS is connected: {c}"));

  return redisSubscriber;
}
```

# How to put it all together
[Try out samples](https://github.com/tomasfabian/Joker/wiki/Samples)

```C#
    private static void CreateReactiveProductsViewModel()
    {
      var reactiveData = new ReactiveData<Product>();
      var redisUrl = ConfigurationManager.AppSettings["RedisUrl"];
      using var entitiesSubscriber = new DomainEntitiesSubscriber<Product>(new RedisSubscriber(redisUrl), reactiveData);

      string connectionString = ConfigurationManager.ConnectionStrings["FargoEntities"].ConnectionString;

      var reactiveProductsViewModel = new ReactiveProductsViewModel(new SampleDbContext(connectionString),
        reactiveData, new WpfSchedulersFactory());

      reactiveProductsViewModel.SubscribeToDataChanges();

      reactiveProductsViewModel.Dispose();
    }
```

[!["Buy Me A Coffee"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/tomasfabian)
