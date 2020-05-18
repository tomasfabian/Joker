using System;
using System.Data.Entity;
using DataAccessLayer.Context;
using Domain.Models;
using Joker.EntityFramework.Repositories;

namespace DataAccessLayer.Repositories
{
  public class BooksRepository : Repository<Book>
  {
    private readonly ISampleDbContext context;

    public BooksRepository(ISampleDbContext context) 
      : base(context)
    {
      this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    protected override IDbSet<Book> DbSet => context.Books;
  }
}