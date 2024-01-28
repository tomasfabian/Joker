using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.OData.Client;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Sample.Domain.Models;

namespace OData.Client
{
  public class ODataServiceContext : DataServiceContext
  {
    #region Constructors

    public ODataServiceContext(Uri serviceRoot, HttpClient httpClient)
      : base(serviceRoot, ODataProtocolVersion.V4)
    {
      if(httpClient != null)
        HttpRequestTransportMode = HttpRequestTransportMode.HttpClient; 
      
      if (EdmModel == null)
      {
        switch (HttpRequestTransportMode)
        {
          case HttpRequestTransportMode.HttpClient: 
            Format.LoadServiceModel = () => GetServiceModelAsync(httpClient).Result;
            break;
          case HttpRequestTransportMode.HttpWebRequest: 
            Format.LoadServiceModel = () => GetServiceModel(GetMetadataUri());
            break;
        }
        
        Format.UseJson();
      }
      else
      {
        Format.UseJson(EdmModel);
      }
    }

    #endregion
    
    public static IEdmModel EdmModel { get; set; }

    //WASM support
    public static async Task<IEdmModel> GetServiceModelAsync(HttpClient httpClient)
    {
      using (var stream = await httpClient.GetStreamAsync("$metadata"))
      using (var reader = XmlReader.Create(stream))
      {
        return EdmModel = CsdlReader.Parse(reader);
      }
    }

    public IEdmModel GetServiceModel(Uri metadataUri)
    {
      var request = WebRequest.CreateHttp(metadataUri);

      using (var response = request.GetResponse())
      using (var stream = response.GetResponseStream())
      using (var reader = XmlReader.Create(stream))
      {
        return CsdlReader.Parse(reader);
      }
    }
    
    private DataServiceQuery<Product> products;

    public DataServiceQuery<Product> Products => products = products ?? CreateQuery<Product>("Products");

    private DataServiceQuery<Author> authors;

    public DataServiceQuery<Author> Authors => authors = authors ?? CreateQuery<Author>("Authors");

    private DataServiceQuery<Book> books;

    public DataServiceQuery<Book> Books => books = books ?? CreateQuery<Book>("Books");
  }
}