using System;
using Microsoft.OData.Client;

namespace ODataIssue1518.Client
{
  public class ODataServiceContext : DataServiceContext
  {
    #region Constructors

    public ODataServiceContext(Uri serviceRoot)
      : base(serviceRoot, ODataProtocolVersion.V4)
    {
    }

    #endregion
  }
}