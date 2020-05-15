using System;
using Joker.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using Sample.DataCore.EFCore;
using Sample.Domain.Models;

namespace Sample.DataCore.Repositories
{
  public class AuthorsRepository : Repository<Author>
  {
    private readonly ISampleDbContext context;

    public AuthorsRepository(ISampleDbContext context) 
      : base(context)
    {
      this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    protected override DbSet<Author> DbSet => context.Authors;
  }
}