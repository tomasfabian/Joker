using System;
using Microsoft.EntityFrameworkCore;
using Sample.DataCore.EFCore;
using Sample.Domain.Models;

namespace SelfHostedODataService.EFCore.Repositories
{
  public class ProductsRepository : RepositoryCore<Product>
  {
    private readonly ISampleDbContext context;

    public ProductsRepository(ISampleDbContext context) 
      : base(context)
    {
      this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    protected override DbSet<Product> DbSet => context.Products;
  }
}