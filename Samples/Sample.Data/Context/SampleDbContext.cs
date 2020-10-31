using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using Joker.EntityFramework.Database;
using Sample.Domain.Models;

namespace Sample.Data.Context
{
  public class SampleDbContext : DbContextBase, ISampleDbContext
  {
    #region Constructors

    static SampleDbContext()
    {
      Database.SetInitializer(new CreateSampleDatabaseIfNotExists());
    }

    public SampleDbContext()
    {
      //Add-Migration Products -verbose
      //Add-Migration Products_Version -StartUpProjectName Joker.WPF.Sample -ProjectName Sample.Data -verbose
      //Add-Migration Products_Version -ConnectionString "Server=127.0.0.1,1401;User Id = SA;Password=<YourNewStrong@Passw0rd>;Initial Catalog = Test;" -ConnectionProviderName "System.Data.SqlClient" -ProjectName Sample.Data -verbose
      //Update-Database -ConnectionString "Server=127.0.0.1,1401;User Id = SA;Password=<YourNewStrong@Passw0rd>;Initial Catalog = Test;" -ConnectionProviderName "System.Data.SqlClient" -ProjectName Sample.Data -verbose
      //Update - Database
      
      //Rollback
      //Update-Database -TargetMigration:"Books_And_Authors" -ConnectionString "Server=127.0.0.1,1401;User Id = SA;Password=<YourNewStrong@Passw0rd>;Initial Catalog = Test;" -ConnectionProviderName "System.Data.SqlClient" -ProjectName Sample.Data.Dev -verbose
    }

    public SampleDbContext(string nameOrConnectionString)
      : base(nameOrConnectionString)
    {
      Configuration.LazyLoadingEnabled = false;
    }

    #endregion

    public void MigrateDatabase(string connectionString)
    {
      if (!Database.Exists() || !Database.CompatibleWithModel(false))
      {      
        Database.SetInitializer(new Migrations.MigrateDatabaseToLatestVersion(connectionString));
        
        Database.Initialize(true);
      }
    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Product>()
        .Property(f => f.Timestamp)
        .HasColumnType("datetime2")
        .HasPrecision(0);
      
      modelBuilder.Entity<Book>()
        .Property(f => f.Timestamp)
        .HasColumnType("datetime2")
        .HasPrecision(0);

      modelBuilder.Entity<Author>()
        .Property(f => f.Timestamp)
        .HasColumnType("datetime2")
        .HasPrecision(0);

      modelBuilder.Entity<Author>()
        .Property(c => c.LastName)
        .HasMaxLength(128)
        .IsRequired();

      modelBuilder.Entity<Author>()
        .HasIndex(b => b.LastName)
        .IsUnique()
        .HasName($"UX_{nameof(Author)}_{nameof(Author.LastName)}");

      modelBuilder.Entity<Publisher>()
        .HasKey(c => new {c.PublisherId1, c.PublisherId2});

      modelBuilder
        .Entity<Publisher>()
        .Property(e => e.PublisherId1)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

      modelBuilder.Entity<Book>()
        .HasOptional(p => p.Publisher)
        .WithMany(c => c.Books)
        .HasForeignKey(p => new {p.PublisherId1, p.PublisherId2});
    }

    public IDbSet<Product> Products { get; set; }

    public IDbSet<Book> Books { get; set; }
    public IDbSet<Author> Authors { get; set; }
    public IDbSet<Publisher> Publishers { get; set; }
  }
}