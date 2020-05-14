using System;
using Sample.Domain.Models;

namespace SelfHostedODataService.EFCore.Repositories
{
  public class AuthorsRepository : RepositoryCore<Author>
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