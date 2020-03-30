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
      return string.IsNullOrEmpty(key) ? this.kernel.Get(serviceType, new IParameter[0]) : this.kernel.Get(serviceType, key, new IParameter[0]);
    }

    protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
    {
      return this.kernel.GetAll(serviceType, new IParameter[0]);
    }
  }
}