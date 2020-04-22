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
      //Add-Migration Products_Version -StartUpProjectName Joker.WPF.Sample -ProjectName Sample.Data -verbose
      //Add-Migration Products_Version -ConnectionString "Server=127.0.0.1,1401;User Id = SA;Password=<YourNewStrong@Passw0rd>;Initial Catalog = Test;" -ConnectionProviderName "System.Data.SqlClient" -ProjectName Sample.Data -verbose
      //Update-Database -ConnectionString "Server=127.0.0.1,1401;User Id = SA;Password=<YourNewStrong@Passw0rd>;Initial Catalog = Test;" -ConnectionProviderName "System.Data.SqlClient" -ProjectName Sample.Data -verbose
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

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Product>()
        .Property(f => f.Timestamp)
        .HasColumnType("datetime2")
        .HasPrecision(0);
    }

    public IDbSet<Product> Products { get; set; }
  }
}