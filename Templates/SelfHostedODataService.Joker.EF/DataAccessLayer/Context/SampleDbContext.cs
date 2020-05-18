using System.Data.Entity;
using Domain.Models;
using Joker.EntityFramework.Database;

namespace DataAccessLayer.Context
{
  public class SampleDbContext : DbContextBase, ISampleDbContext
  {
    #region Constructors

    public SampleDbContext()
    {
    }

    public SampleDbContext(string nameOrConnectionString)
      : base(nameOrConnectionString)
    {
      Configuration.LazyLoadingEnabled = false;
    }

    #endregion

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Book>()
        .Property(f => f.Timestamp)
        .HasColumnType("datetime2")
        .HasPrecision(0);
    }

    public IDbSet<Book> Books { get; set; }
  }
}