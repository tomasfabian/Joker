using System;
using System.Net;
using System.Xml;
using Microsoft.OData.Client;
using Microsoft.OData.Edm;
using Sample.Domain.Models;

namespace OData.Client
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
  }
}