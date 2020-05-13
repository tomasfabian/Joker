using Microsoft.EntityFrameworkCore;
using Sample.Domain.Models;

namespace Sample.DataCore.EFCore
{
  public class SampleDbContextCore : DbContext, ISampleDbContext
  {
    #region Constructors

    public SampleDbContextCore(DbContextOptions<SampleDbContextCore> options) 
      : base(options)
    {
      // Database.EnsureCreated();
    }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Product>()
        .Property(f => f.Timestamp);
        // .HasColumnType("datetime2")
        // .HasPrecision(0);

      modelBuilder.Entity<Book>()
        .Property(f => f.Timestamp);

      modelBuilder.Entity<Author>()
        .Property(f => f.Timestamp);

      modelBuilder.Entity<Author>()
        .Property(c => c.LastName)
        .HasMaxLength(128)
        .IsRequired();

      modelBuilder.Entity<Author>()
        .HasIndex(b => b.LastName)
        .IsUnique();

      modelBuilder.Entity<Publisher>()
        .HasKey(c => new { c.PublisherId1, c.PublisherId2 });

      modelBuilder
        .Entity<Publisher>()
        .Property(e => e.PublisherId1)
        .ValueGeneratedOnAdd();
        // .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

      modelBuilder.Entity<Book>()
        // .HasOptional(p => p.Publisher)
        .HasOne(p => p.Publisher)
        .WithMany(c => c.Books)
        .HasForeignKey(p => new { p.PublisherId1, p.PublisherId2 });
    }

    public DbSet<Product> Products { get; set; }

    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
  }
}