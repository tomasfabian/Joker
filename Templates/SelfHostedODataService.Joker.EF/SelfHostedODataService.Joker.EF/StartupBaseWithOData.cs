using System;
using Autofac;
using AutoMapper;
using Domain.Models;
using Joker.Factories.Schedulers;
using Joker.OData.Extensions.OData;
using Joker.OData.Startup;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SelfHostedODataService.Joker.EF.AutofacModules;
using SelfHostedODataService.Joker.EF.Configuration;

namespace SelfHostedODataService.Joker.EF
{
  public class StartupBaseWithOData : ODataStartup
  {
    public StartupBaseWithOData(IWebHostEnvironment env)
      : base(env)
    {
    }

    protected override ODataModelBuilder OnCreateEdmModel(ODataModelBuilder oDataModelBuilder)
    {
      oDataModelBuilder.Namespace = "Default";

      oDataModelBuilder.AddPluralizedEntitySet<Book>();

      return oDataModelBuilder;
    }

    protected override void OnConfigureServices(IServiceCollection services)
    {
      base.OnConfigureServices(services);

      services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }

    protected override void RegisterTypes(ContainerBuilder builder)
    {
      base.RegisterTypes(builder);

      ContainerBuilder.RegisterModule(new AutofacModule());

      ContainerBuilder.RegisterType<ConfigurationProvider>()
        .As<Configuration.IConfigurationProvider>()
        .SingleInstance();

      ContainerBuilder.RegisterType<SchedulersFactory>().As<ISchedulersFactory>()
        .SingleInstance();
    }
  }
}