using System;
using DataAccessLayer.EFCore;
using Domain.Models;
using Joker.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

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

    protected override DbSet<Book> DbSet => context.Books;
  }
}