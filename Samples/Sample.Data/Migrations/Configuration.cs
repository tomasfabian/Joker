using Sample.Domain.Models;

namespace Sample.Data.Migrations
{
  using System;
  using System.Data.Entity;
  using System.Data.Entity.Migrations;
  using System.Linq;

  internal sealed class Configuration : DbMigrationsConfiguration<Context.SampleDbContext>
  {
    public Configuration()
    {
      AutomaticMigrationsEnabled = true;
    }

    protected override void Seed(Context.SampleDbContext context)
    {
      //  This method will be called after migrating to the latest version.

      //  You can use the DbSet<T>.AddOrUpdate() helper extension method
      //  to avoid creating duplicate seed data.
      context.Products.Add(new Product {Timestamp = DateTime.Now, Name = "Test product"});

      context.Authors.Add(new Author { Timestamp = DateTime.Now, LastName = "Sheldrake" });

      context.SaveChanges();
    }
  }
}