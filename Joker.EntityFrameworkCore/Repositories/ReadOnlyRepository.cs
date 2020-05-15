using System;
using System.Linq;
using Joker.Contracts.Data;
using Microsoft.EntityFrameworkCore;

namespace Joker.EntityFrameworkCore.Repositories
{
  public abstract class ReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity>
    where TEntity : class
  {
    private readonly IContext context;

    protected ReadOnlyRepository(IContext context)
    {
      this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    protected abstract DbSet<TEntity> DbSet { get; }

    public IQueryable<TEntity> GetAll()
    {
      return DbSet;
    }

    public IQueryable<TEntity> GetAllIncluding(string path)
    {
      return DbSet.Include(path);
    }
  }
}