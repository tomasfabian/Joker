using System.Reactive.Concurrency;
using Autofac;
using Autofac.Core;
using Joker.Contracts.Data;
using Joker.Extensions;
using Joker.Factories.Schedulers;
using Joker.Redis.ConnectionMultiplexers;
using Joker.Redis.SqlTableDependency;
using Sample.Data.Context;
using Sample.Data.SqlTableDependencyProvider;
using Sample.Domain.Models;
using SelfHostedODataService.Configuration;
using SelfHostedODataService.Redis;
using SelfHostedODataService.Repositories;
using SqlTableDependency.Extensions;

namespace SelfHostedODataService.AutofacModules
{
  public class ProductsAutofacModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      base.Load(builder);

      var connectionStringParameter = new ResolvedParameter(
        (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name.IsOneOfFollowing("nameOrConnectionString", "connectionString"),
        (pi, ctx) => ctx.Resolve<IProductsConfigurationProvider>().GetDatabaseConnectionString());

      builder.RegisterType<SampleDbContext>().As<ISampleDbContext>()
        .WithParameter(connectionStringParameter)
        .InstancePerLifetimeScope();

      builder.Register<IScheduler>(c => c.Resolve<ISchedulersFactory>().TaskPool)
        .SingleInstance();

      builder.RegisterType<ProductsSqlTableDependencyProvider>()
        .As<ISqlTableDependencyProvider<Product>>()
        .WithParameter(connectionStringParameter)
        .WithParameter("lifetimeScope", SqlTableDependency.Extensions.Enums.LifetimeScope.UniqueScope)
        .SingleInstance();

      var redisUrlParameter = new ResolvedParameter(
        (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == "url",
        (pi, ctx) => ctx.Resolve<IProductsConfigurationProvider>().RedisUrl);

      builder.RegisterType<RedisPublisher>().As<IRedisPublisher>()
        .WithParameter(redisUrlParameter);

      builder.RegisterType<ProductSqlTableDependencyRedisProvider>()
        .As<ISqlTableDependencyRedisProvider<Product>>();

      RegisterRepositories(builder);
    }

    private static void RegisterRepositories(ContainerBuilder builder)
    {
      builder.RegisterType<ProductsRepository>()
        .As<IRepository<Product>>()
        .InstancePerLifetimeScope();

      builder.RegisterType<AuthorsRepository>()
        .As<IRepository<Author>>()
        .InstancePerLifetimeScope();

      builder.RegisterType<BooksRepository>()
        .As<IRepository<Book>, IReadOnlyRepository<Book>>()
        .InstancePerLifetimeScope();
    }
  }
}