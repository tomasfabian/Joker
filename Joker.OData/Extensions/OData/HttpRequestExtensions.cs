using System;
using System.Collections.Generic;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.OData.UriParser;

namespace Joker.OData.Extensions.OData
{
  public static class HttpRequestExtensions
  {    
    public static Microsoft.AspNet.OData.Routing.ODataPath CreateODataPath(this HttpRequest request, Uri uri)
    {
      var pathHandler = request.GetPathHandler();

      var serviceRoot = request.GetUrlHelper().CreateODataLink(
        request.ODataFeature().RouteName,
        pathHandler,
        new List<ODataPathSegment>());

      return pathHandler.Parse(serviceRoot, uri.LocalPath, request.GetRequestContainer());
    }
  }
}