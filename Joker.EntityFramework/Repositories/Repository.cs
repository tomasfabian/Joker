using System;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using Joker.Contracts.Data;

namespace Joker.EntityFramework.Repositories
{
  public abstract class Repository<TEntity> : ReadOnlyRepository<TEntity>, IRepository<TEntity> 
    where TEntity : class
  {
    private readonly IContext context;

    protected Repository(IContext context)
      : base(context)
    {
      this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Add(TEntity entity)
    {
      DbSet.Add(entity);
    }

    public void Update(TEntity entity)
    {
      DbSet.AddOrUpdate(entity);
    }

    public void Remove(params object[] keys)
    {
      var entity = DbSet.Find(keys);
      
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