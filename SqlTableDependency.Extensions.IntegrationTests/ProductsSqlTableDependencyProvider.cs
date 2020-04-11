using System;
using System.Configuration;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Sample.Domain.Models;
using SqlTableDependency.Extensions.Enums;

namespace SqlTableDependency.Extensions.IntegrationTests
{
  public class ProductsSqlTableDependencyProvider : SqlTableDependencyProvider<Product>
  {
    public ProductsSqlTableDependencyProvider(ConnectionStringSettings connectionStringSettings, IScheduler scheduler, LifetimeScope lifetimeScope) 
      : base(connectionStringSettings, scheduler, lifetimeScope)
    {
    }

    public ProductsSqlTableDependencyProvider(string connectionString, IScheduler scheduler, LifetimeScope lifetimeScope) 
      : base(connectionString, scheduler, lifetimeScope)
    {
    }

    #region TableName

    protected override string TableName => base.TableName+"s";

    #endregion
    
    public override TimeSpan ReconnectionTimeSpan => TimeSpan.FromSeconds(3);

    public Product LastInsertedProduct { get; private set; }

    private readonly ISubject<Product> lastInsertedProductSubject = new ReplaySubject<Product>(1);
    public IObservable<Product> LastInsertedProductChanged => lastInsertedProductSubject.AsObservable();

    protected override void OnInserted(Product entity)
    {
      base.OnInserted(entity);

      LastInsertedProduct = entity;

      lastInsertedProductSubject.OnNext(entity);
    }
    
    public Product LastUpdatedProduct { get; private set; }

    private readonly ISubject<Product> lastUpdatedProductSubject = new ReplaySubject<Product>(1);
    public IObservable<Product> LastUpdatedProductChanged => lastUpdatedProductSubject.AsObservable();

    protected override void OnUpdated(Product entity)
    {
      base.OnUpdated(entity);

      LastUpdatedProduct = entity;

      lastUpdatedProductSubject.OnNext(entity);
    }
    
    public Product LastDeletedProduct { get; private set; }

    private readonly ISubject<Product> lastDeletedProductSubject = new ReplaySubject<Product>(1);
    public IObservable<Product> LastDeletedProductChanged => lastDeletedProductSubject.AsObservable();

    protected override void OnDeleted(Product entity)
    {
      base.OnDeleted(entity);

      LastDeletedProduct = entity;

      lastDeletedProductSubject.OnNext(entity);
    }
    
    public Exception LastException { get; private set; }

    private readonly ISubject<Exception> lastExceptionSubject = new ReplaySubject<Exception>(2);
    public IObservable<Exception> LastExceptionChanged => lastExceptionSubject.AsObservable();

    protected override void OnError(Exception error)
    {
      base.OnError(error);

      LastException = error;

      lastExceptionSubject.OnNext(error);
    }

    protected override void OnConnected()
    {
      base.OnConnected();

    }
  }
}