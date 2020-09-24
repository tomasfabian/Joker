using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Joker.EntityFrameworkCore.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sample.Domain.Models;

namespace Sample.DataCore.EFCore
{
  public class SampleDbContextCore : DbContextBase, ISampleDbContext
  {
    #region Constructors

    public SampleDbContextCore()
    {
    }

    public SampleDbContextCore(DbContextOptions<SampleDbContextCore> options)
      : base(options)
    {
    }

    //Install-Package Microsoft.EntityFrameworkCore.Tools
    //add reference to Microsoft.EntityFrameworkCore.SqlServer
    //add Microsoft.EntityFrameworkCore.Design reference to startup project
    //set SelfHostedODataService.EFCore.Dev.csproj as startup project
    //set package manager console default project to Sample.DataCore.Dev
    //Add-Migration TestDb -verbose
    //Update-Database

    #endregion

    #region Properties

    public override ChangeTracker ChangeTracker
    {
      get
      {
        var changeTracker = base.ChangeTracker;

        changeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        changeTracker.LazyLoadingEnabled = false;

        return changeTracker;
      }
    }

    public DbSet<Product> Products { get; set; }

    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Publisher> Publishers { get; set; }

    #endregion

    #region Methods

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Product>()
        .Property(f => f.Timestamp)
        .HasDefaultValueSql("GetDate()");

      modelBuilder.Entity<Product>().HasData(
        new Product { Id = -1, Name = "Test" });

      modelBuilder.Entity<Author>()
        .Property(f => f.Timestamp);

      modelBuilder.Entity<Author>()
        .Property(c => c.LastName)
        .HasMaxLength(128)
        .IsRequired();

      modelBuilder.Entity<Author>()
        .HasIndex(b => b.LastName)
        .IsUnique();

      modelBuilder.Entity<Book>()
        .Property(f => f.Timestamp)
        .HasDefaultValueSql("GetDate()");

      modelBuilder.Entity<Publisher>()
        .HasKey(c => new {c.PublisherId1, c.PublisherId2});

      var publisherEntity = modelBuilder
        .Entity<Publisher>();
      
      publisherEntity.Property(e => e.PublisherId1)
        .ValueGeneratedOnAdd();

      modelBuilder.Entity<Book>()
        .HasOne(p => p.Publisher)
        .WithMany(c => c.Books)
        .HasForeignKey(p => new {p.PublisherId1, p.PublisherId2});

      modelBuilder.Entity<Publisher>().HasData(new Publisher { PublisherId1 = -1, PublisherId2 = -1, Title = "Publisher 1" });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
      InterceptSaveChanges();

      return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
      InterceptSaveChanges();

      return base.SaveChanges();
    }

    private void InterceptSaveChanges()
    {
      var entries = ChangeTracker
        .Entries()
        .Where(e => e.Entity is Product && e.State == EntityState.Modified);

      foreach (var entityEntry in entries)
      {
        ((Product)entityEntry.Entity).Timestamp =
          DateTime.Now; // SQL Server GetDate() may be different in production use rather db trigger
      }
    }

    #endregion
  }
}