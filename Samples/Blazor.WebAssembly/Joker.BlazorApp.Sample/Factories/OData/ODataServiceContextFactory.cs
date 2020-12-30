using System;
using Microsoft.Extensions.Configuration;
using OData.Client;
using System.Net.Http;

namespace Joker.BlazorApp.Sample.Factories.OData
{
  public class ODataServiceContextFactory : IODataServiceContextFactory
  {
    private readonly HttpClient httpClient;
    private readonly IConfiguration configuration;

    public ODataServiceContextFactory(HttpClient httpClient, IConfiguration configuration)
    {
      this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
      this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
  
    public ODataServiceContext Create(string url)
    {
      return new ODataServiceContext(new Uri(url), httpClient);
    }

    public ODataServiceContext CreateODataContext()
    {
      var odataUrl = configuration["ODataUrl"];

      var dataServiceContext = Create(odataUrl);

      return dataServiceContext;
    }
  }
}