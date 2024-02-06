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
}