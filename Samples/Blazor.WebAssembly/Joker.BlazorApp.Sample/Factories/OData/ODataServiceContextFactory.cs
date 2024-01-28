using OData.Client;

namespace Joker.BlazorApp.Sample.Factories.OData
{
  public class ODataServiceContextFactory(HttpClient httpClient, IConfiguration configuration)
    : IODataServiceContextFactory
  {
    private readonly HttpClient httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly IConfiguration configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

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