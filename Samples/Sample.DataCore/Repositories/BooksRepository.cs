using System;
using Joker.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using Sample.DataCore.EFCore;
using Sample.Domain.Models;

namespace Sample.DataCore.Repositories
{
  public class BooksRepository : Repository<Book>
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