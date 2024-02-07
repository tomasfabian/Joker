using Microsoft.OData.UriParser;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Extensions;

namespace Joker.OData.Extensions.Http;

public static class HttpRequestExtensions
{
  public static ODataPath GetODataPath(this HttpRequest request, Uri link)
  {
    var edmModel = request.GetModel();

    var baseAddress = request.ODataFeature().BaseAddress;
    var oDataUriParser = new ODataUriParser(edmModel, new Uri(baseAddress), link);
    var odataPath = oDataUriParser.ParsePath();
    return odataPath;
  }

  public static object[] GetAllKeysFromODataPath(this HttpRequest request)
  {
    var link = request.Query["$id"];
    var oDataFeature = request.ODataFeature();
    var uriBuilder = new UriBuilder(request.Scheme, request.Host.Host, request.Host.Port ?? -1);
    var baseAddress = uriBuilder.Uri + oDataFeature.RoutePrefix;
    var oDataUriParser = new ODataUriParser(request.GetModel(), new Uri(baseAddress), new Uri(link));
    var odataPath = oDataUriParser.ParsePath();
    return ODataPathHelpers.GetKeysFromPath(odataPath);
  }

  public static object[] GetKeysFromODataPath(this HttpRequest request)
  {
    var odataPath = request.ODataFeature().Path;

    var value = ODataPathHelpers.GetKeysFromPath(odataPath);

    return value;
  }
}