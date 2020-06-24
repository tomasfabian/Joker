namespace OData.Client
{
  public interface IODataServiceContextFactory
  {
    ODataServiceContext Create(string url);
    ODataServiceContext CreateODataContext();
  }
}