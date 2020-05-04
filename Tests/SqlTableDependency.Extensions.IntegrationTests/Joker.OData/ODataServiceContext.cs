using System;
using System.Net;
using System.Xml;
using Microsoft.OData.Client;
using Microsoft.OData.Edm;
using Sample.Domain.Models;

namespace SqlTableDependency.Extensions.IntegrationTests.Dev.Joker.OData
{
  public class ODataServiceContext : DataServiceContext
  {
    #region Constructors

    public ODataServiceContext(Uri serviceRoot)
      : base(serviceRoot, ODataProtocolVersion.V4)
    {         
      

      Format.LoadServiceModel = () => GetServiceModel(GetMetadataUri());
      Format.UseJson();
    }

    #endregion

    public IEdmModel GetServiceModel(Uri metadataUri)
    {
      var request = WebRequest.CreateHttp(metadataUri);

      using (var response = request.GetResponse())
      using (var stream = response.GetResponseStream())
      using (var reader = XmlReader.Create(stream))
      {
        return Microsoft.OData.Edm.Csdl.CsdlReader.Parse(reader);
      }
    }

    private DataServiceQuery<Product> products;

    public DataServiceQuery<Product> Products => products = products ?? CreateQuery<Product>("Products");

    private DataServiceQuery<Author> authors;

    public DataServiceQuery<Author> Authors => authors = authors ?? CreateQuery<Author>("Authors");

    private DataServiceQuery<Book> books;

    public DataServiceQuery<Book> Books => books = books ?? CreateQuery<Book>("Books");

    private DataServiceQuery<Publisher> publishers;

    public DataServiceQuery<Publisher> Publishers => publishers = publishers ?? CreateQuery<Publisher>("Publishers");
  }
}