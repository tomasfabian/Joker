using System;

namespace OData.Client
{
  public class ODataServiceContextFactory
  {
    public ODataServiceContext Create(string url)
    {
      return new ODataServiceContext(new Uri(url));
    }
  }
}