using System;
using System.Data.Entity;
using Joker.OData.Repositories;
using Sample.Data.Context;
using Sample.Domain.Models;

namespace SelfHostedODataService.Repositories
{
  public class ProductsRepository : Repository<Product>
  {
    private readonly ISampleDbContext context;

    public ProductsRepository(ISampleDbContext context) 
      : base(context)
    {
      this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    protected override IDbSet<Product> DbSet => context.Products;
  }
}