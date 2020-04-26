using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Joker.Contracts.Data;

namespace Joker.OData.Repositories
{
  public abstract class Repository<TEntity> : IRepository<TEntity> 
    where TEntity : class
  {
    private readonly IContext context;

    protected Repository(IContext context)
    {
      this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    protected abstract IDbSet<TEntity> DbSet { get; }

    public IQueryable<TEntity> GetAll()
    {
      return DbSet;
    }

    public void Add(TEntity entity)
    {
      DbSet.Add(entity);
    }

    public void Update(TEntity entity)
    {
      DbSet.AddOrUpdate(entity);
    }

    public void Remove(int key)
    {
      var entity = DbSet.Find(key);
      
      DbSet.Remove(entity);
    }

    public void Remove(TEntity entity)
    {
      DbSet.Remove(entity);
    }

    public Task<int> SaveChangesAsync()
    {
      return context.SaveChangesAsync();
    }
  }
}