using System;
using System.Data.Entity;
using Joker.EntityFramework.Repositories;
using Sample.Data.Context;
using Sample.Domain.Models;

namespace SelfHostedODataService.Repositories
{
  public class PublishersRepository : Repository<Publisher>
  {
    private readonly ISampleDbContext context;

    public PublishersRepository(ISampleDbContext context) 
      : base(context)
    {
      this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    protected override IDbSet<Publisher> DbSet => context.Publishers;
  }
}