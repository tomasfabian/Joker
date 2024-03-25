using System;
using System.Collections.Generic;
using CommonServiceLocator;
using Ninject;
using Ninject.Parameters;

namespace Joker.WPF.Sample.Modularity
{
  public class NinjectServiceLocator : ServiceLocatorImplBase
  {
    private readonly IKernel kernel;

    public NinjectServiceLocator(IKernel kernel)
    {
      this.kernel = kernel;
    }

    protected override object DoGetInstance(Type serviceType, string key)
    {
      return string.IsNullOrEmpty(key) ? kernel.Get(serviceType, Array.Empty<IParameter>()) : kernel.Get(serviceType, key, Array.Empty<IParameter>());
    }

    protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
    {
      return kernel.GetAll(serviceType, Array.Empty<IParameter>());
    }
  }
}