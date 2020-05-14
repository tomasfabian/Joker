using System;
using Sample.Domain.Models;

namespace SelfHostedODataService.EFCore.Repositories
{
  public class BooksRepository : RepositoryCore<Book>
  {
    private readonly ISampleDbContext context;

    public BooksRepository(ISampleDbContext context) 
      : base(context)
    {
      this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    protected override DbSet<Book> DbSet => context.Books;
  }
}