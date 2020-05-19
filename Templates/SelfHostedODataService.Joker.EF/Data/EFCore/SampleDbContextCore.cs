using Domain.Models;
using Joker.EntityFrameworkCore.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DataAccessLayer.EFCore
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

    public DbSet<Book> Books { get; set; }

    #endregion

    #region Methods

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Book>()
        .Property(f => f.Timestamp)
        .HasDefaultValueSql("GetDate()");
    }

    #endregion
  }
}