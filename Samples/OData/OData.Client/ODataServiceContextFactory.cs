using System;
using System.Configuration;

namespace OData.Client
{
  public class ODataServiceContextFactory
  {
    public ODataServiceContext Create(string url)
    {
      return new ODataServiceContext(new Uri(url));
    }

    public ODataServiceContext CreateODataContext()
    {
      var odataUrl = ConfigurationManager.AppSettings["ODataUrl"];

      var dataServiceContext = Create(odataUrl);

      return dataServiceContext;
    }
  }
}