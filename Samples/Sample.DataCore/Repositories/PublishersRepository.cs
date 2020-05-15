using System;
using Joker.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using Sample.DataCore.EFCore;
using Sample.Domain.Models;

namespace Sample.DataCore.Repositories
{
  public class PublishersRepository : Repository<Publisher>
  {
    private readonly ISampleDbContext context;

    public PublishersRepository(ISampleDbContext context) 
      : base(context)
    {
      this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    protected override DbSet<Publisher> DbSet => context.Publishers;
  }
}