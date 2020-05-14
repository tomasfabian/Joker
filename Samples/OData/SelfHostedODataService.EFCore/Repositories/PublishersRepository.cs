using System;
using Sample.Domain.Models;

namespace SelfHostedODataService.EFCore.Repositories
{
  public class PublishersRepository : RepositoryCore<Publisher>
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