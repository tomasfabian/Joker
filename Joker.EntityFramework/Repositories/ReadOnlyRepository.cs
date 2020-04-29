using System;
using System.Data.Entity;
using System.Linq;
using Joker.Contracts.Data;

namespace Joker.EntityFramework.Repositories
{
  public abstract class ReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity>
    where TEntity : class
  {
    private readonly IContext context;

    protected ReadOnlyRepository(IContext context)
    {
      this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    protected abstract IDbSet<TEntity> DbSet { get; }

    public IQueryable<TEntity> GetAll()
    {
      return DbSet;
    }
  }
}