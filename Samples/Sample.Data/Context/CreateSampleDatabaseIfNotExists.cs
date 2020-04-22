using System;
using System.Data.Entity;
using Sample.Domain.Models;

namespace Sample.Data.Context
{
  public class CreateSampleDatabaseIfNotExists : CreateDatabaseIfNotExists<SampleDbContext>
  {
    protected override void Seed(SampleDbContext context)
    {
      base.Seed(context);
      
      context.Products.Add(new Product {Timestamp = DateTime.Now, Name = "Test product"});

      context.SaveChanges();
    }
  }
}