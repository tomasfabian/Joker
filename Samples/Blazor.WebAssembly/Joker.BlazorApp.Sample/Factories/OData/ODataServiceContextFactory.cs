using System;
using Microsoft.Extensions.Configuration;
using Microsoft.OData.Extensions.Client;
using OData.Client;

namespace Joker.BlazorApp.Sample.Factories.OData
{
  public class ODataServiceContextFactory : IODataServiceContextFactory
  {
    private readonly IODataClientFactory clientFactory;
    private readonly IConfiguration configuration;

    public ODataServiceContextFactory(IODataClientFactory clientFactory, IConfiguration configuration)
    {
      this.clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
      this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public ODataServiceContext Create(string url)
    {
      return clientFactory.CreateClient<ODataServiceContext>(new Uri(url));
    }

    public ODataServiceContext CreateODataContext()
    {
      var odataUrl = configuration["ODataUrl"];
      
      var dataServiceContext = Create(odataUrl);

      return dataServiceContext;
    }
  }
}