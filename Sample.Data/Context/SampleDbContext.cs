using System.Data.Entity;
using Sample.Domain.Models;

namespace Sample.Data.Context
{
  public class SampleDbContext : DbContext
  {
    #region Constructors

    public SampleDbContext()
    {
      //Add-Migration Products -verbose
      //Update - Database
    }

    public SampleDbContext(string nameOrConnectionString)
      : base(nameOrConnectionString)
    {
      Database.SetInitializer(new CreateSampleDatabaseIfNotExists());
    }

    #endregion

    public IDbSet<Product> Products { get; set; }
  }
}