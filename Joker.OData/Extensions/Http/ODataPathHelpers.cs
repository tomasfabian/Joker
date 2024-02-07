using System;
using System.Linq;
using Microsoft.OData.UriParser;

namespace Joker.OData.Extensions.Http;

internal static class ODataPathHelpers
{
  internal static object[] GetKeysFromPath(ODataPath odataPath)
  {
    var keySegment = odataPath.OfType<KeySegment>().FirstOrDefault();
    if (keySegment == null)
      throw new InvalidOperationException("The link does not contain a key.");

    var value = keySegment.Keys.Select(c => c.Value).ToArray();

    return value;
  }
}