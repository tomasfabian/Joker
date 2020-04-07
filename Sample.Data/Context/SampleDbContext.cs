using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using Sample.Domain.Models;

namespace Sample.Data.Context
{
  public class SampleDbContext : DbContext, ISampleDbContext
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
    
    public SampleDbContext(string connectionString, DbCompiledModel model)
      : base(connectionString, model)
    {
    }

    public SampleDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext)
      : base(objectContext, dbContextOwnsObjectContext)
    {

    }

    #endregion

    public IDbSet<Product> Products { get; set; }
  }
}