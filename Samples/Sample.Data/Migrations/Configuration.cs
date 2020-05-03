using Sample.Data.Context;
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
      AutomaticMigrationDataLossAllowed = true;
    }

    protected override void Seed(Context.SampleDbContext context)
    {
      base.Seed(context);
      //  This method will be called after migrating to the latest version.

      //  You can use the DbSet<T>.AddOrUpdate() helper extension method
      //  to avoid creating duplicate seed data.
      ApplySeed(context);
    }

    public static void ApplySeed(SampleDbContext context)
    {
      context.Products.AddOrUpdate(c => c.Name, new Product {Timestamp = DateTime.Now, Name = "Test product"});

      context.Authors.AddOrUpdate(c => c.LastName, new Author {Timestamp = DateTime.Now, LastName = "Sheldrake"});

      context.Publishers.AddOrUpdate(c => new { c.PublisherId1, c.PublisherId2 }, new Publisher { PublisherId1 = 1, PublisherId2 = 0, Title = "Publisher" });

      context.SaveChanges();
    }
  }
}