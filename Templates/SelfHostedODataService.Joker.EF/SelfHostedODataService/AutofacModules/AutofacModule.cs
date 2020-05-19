using System.Reactive.Concurrency;
using Autofac;
using Autofac.Core;
using DataAccessLayer.EFCore;
using DataAccessLayer.Repositories;
using Domain.Models;
using Joker.Contracts.Data;
using Joker.Extensions;
using Joker.Factories.Schedulers;
using SelfHostedODataService.Configuration;

namespace SelfHostedODataService.AutofacModules
{
  public class AutofacModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      base.Load(builder);

      var connectionStringParameter = new ResolvedParameter(
        (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name.IsOneOfFollowing("nameOrConnectionString", "connectionString"),
        (pi, ctx) => ctx.Resolve<IConfigurationProvider>().GetDatabaseConnectionString());

      builder.RegisterType<SampleDbContextCore>()
        .As<ISampleDbContext, IDbTransactionFactory, IContext>()
        .WithParameter(connectionStringParameter)
        .InstancePerLifetimeScope();

      builder.Register<IScheduler>(c => c.Resolve<ISchedulersFactory>().TaskPool)
        .SingleInstance();

      RegisterRepositories(builder);
    }

    private static void RegisterRepositories(ContainerBuilder builder)
    {
      builder.RegisterType<BooksRepository>()
        .As<IRepository<Book>, IReadOnlyRepository<Book>>()
        .InstancePerLifetimeScope();
    }
  }
}