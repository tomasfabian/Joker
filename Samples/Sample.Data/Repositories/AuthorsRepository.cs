using System;
using System.Data.Entity;
using Joker.EntityFramework.Repositories;
using Sample.Data.Context;
using Sample.Domain.Models;

namespace Sample.Data.Repositories
{
  public class AuthorsRepository : Repository<Author>
  {
    private readonly ISampleDbContext context;

    public AuthorsRepository(ISampleDbContext context) 
      : base(context)
    {
      this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    protected override IDbSet<Author> DbSet => context.Authors;
  }
}