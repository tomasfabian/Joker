using System;
using System.Linq;
using Joker.Contracts.Data;
using Microsoft.EntityFrameworkCore;

namespace Sample.DataCore.EFCore
{
  public abstract class ReadOnlyRepositoryCore<TEntity> : IReadOnlyRepository<TEntity>
    where TEntity : class
  {
    private readonly IContext context;

    protected ReadOnlyRepositoryCore(IContext context)
    {
      this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    protected abstract DbSet<TEntity> DbSet { get; }

    public IQueryable<TEntity> GetAll()
    {
      return DbSet;
    }
  }
}