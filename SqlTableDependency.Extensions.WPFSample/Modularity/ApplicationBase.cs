using System;
using System.Windows;
using CommonServiceLocator;
using Ninject;
using Prism;
using Prism.Ioc;
using Prism.Ninject.Ioc;

namespace SqlTableDependency.Extensions.WPFSample.Modularity
{
  public class ApplicationBase : PrismApplicationBase
  {
    private readonly IKernel kernel = new StandardKernel();

    #region CreateContainerExtension

    protected override IContainerExtension CreateContainerExtension()
    {
      return new NinjectContainerExtension(kernel);
    }

    #endregion    
    
    #region ConfigureServiceLocator

    protected override void ConfigureServiceLocator()
    {
      kernel.Bind<IServiceLocator>().To<NinjectServiceLocator>();

      ServiceLocator.SetLocatorProvider(() => kernel.Get<IServiceLocator>());
    }

    #endregion

    #region RegisterTypes

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
    }

    #endregion

    #region CreateShell

    protected override Window CreateShell()
    {
      return new Shell();
    }

    #endregion
  }
}